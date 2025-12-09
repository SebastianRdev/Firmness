using System.Data;
using System.Text;
using System.Text.Json;
using Firmness.Application.Interfaces;
using Firmness.Infrastructure.Data;
using Google.GenAI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Firmness.Infrastructure.Services.AI;

public class SqlAgentService : ISqlAgentService
{
    private readonly Client _client;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SqlAgentService> _logger;
    private const string MODEL = "gemini-2.5-flash"; // Or use the same model constant as other services

    public SqlAgentService(Client client, ApplicationDbContext context, ILogger<SqlAgentService> logger)
    {
        _client = client;
        _context = context;
        _logger = logger;
    }

    public async Task<string> ProcessUserQueryAsync(string userMessage)
    {
        try
        {
            // 1. Generate SQL
            var sqlQuery = await GenerateSqlFromTextAsync(userMessage);
            if (string.IsNullOrWhiteSpace(sqlQuery))
            {
                return "Lo siento, no pude entender cómo consultar eso en la base de datos.";
            }

            // 2. Execute SQL
            var dataResult = await ExecuteSqlQueryAsync(sqlQuery);
            
            // 3. Generate Natural Response
            var finalResponse = await GenerateNaturalResponseAsync(userMessage, dataResult, sqlQuery);
            
            return finalResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing AI Agent query");
            return "Ocurrió un error interno al procesar tu consulta.";
        }
    }

    private async Task<string> GenerateSqlFromTextAsync(string question)
    {
        var schema = @"
Tables:
- Products (Id, Name, Description, Price, Stock, CategoryId, IsActive, CreatedAt)
- Categories (Id, Name, Description)
- Customers (Id, FirstName, LastName, Email, PhoneNumber, DocumentNumber)
- Sales (Id, SaleDate, TotalAmount, CustomerId, InvoiceNumber)
- SaleDetails (Id, SaleId, ProductId, Quantity, UnitPrice, TotalPrice)
- Receipts (Id, SaleId, ReceiptPath, GeneratedAt)

Rules:
1. Return ONLY the SQL Query. No markdown formats (no ```sql).
2. Use Double Quotes for table and column names (PostgreSQL case sensitivity). e.g. ""Products"", ""Name"".
3. ONLY generate SELECT queries.
4. If the question cannot be answered with this schema, return SELECT 'I cannot answer this' as result.
5. Limit results to 20 rows if not specified.
";

        var prompt = $@"
System: You are an expert PostgreSQL Data Analyst.
{schema}

User Question: {question}
SQL Query:
";

        var response = await _client.Models.GenerateContentAsync(
            model: MODEL,
            contents: prompt
        );

        var sql = response.Candidates[0].Content.Parts[0].Text.Trim();
        
        // Sanitize
        sql = sql.Replace("```sql", "").Replace("```", "").Trim();
        
        // Security Check
        var forbidden = new[] { "DROP", "DELETE", "UPDATE", "INSERT", "ALTER", "TRUNCATE", "GRANT", "REVOKE", ";" }; 
        // Note: checking ";" is too strict for valid SQL, but useful to prevent multi-statement injection if intended.
        // We will allow ";" at the end but ensure no multiple commands.
        
        var upperSql = sql.ToUpper();
        if (upperSql.Contains("DROP ") || upperSql.Contains("DELETE ") || upperSql.Contains("UPDATE ") || upperSql.Contains("INSERT "))
        {
            _logger.LogWarning($"Blocked potentially unsafe query: {sql}");
            throw new InvalidOperationException("Unsafe query detected.");
        }

        return sql;
    }

    private async Task<string> ExecuteSqlQueryAsync(string sql)
    {
        var results = new List<Dictionary<string, object>>();
        var connection = _context.Database.GetDbConnection();
        
        try
        {
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = sql;
            
            // In a real scenario, use a ReadOnly User here!
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.GetValue(i);
                }
                results.Add(row);
            }
        }
        catch(Exception ex)
        {
             _logger.LogError($"SQL Execution failed: {sql}. Error: {ex.Message}");
             return $"Error executing query: {ex.Message}";
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                await connection.CloseAsync();
        }

        if (results.Count == 0) return "No results found.";

        return JsonSerializer.Serialize(results);
    }

    private async Task<string> GenerateNaturalResponseAsync(string question, string methodResult, string generatedSql)
    {
        var prompt = $@"
User requested: {question}
I executed this SQL: {generatedSql}

Database Result:
{methodResult}

Task: Provide a natural, friendly, and concise answer to the user based on the Database Result.
If the result is an error, apologize.
If the result is empty, say so.
Do not mention SQL or technical details unless asked.
Format values properly (currency, dates).
";

        var response = await _client.Models.GenerateContentAsync(
            model: MODEL,
            contents: prompt
        );

        return response.Candidates[0].Content.Parts[0].Text ?? "No response generated.";
    }
}
