using Google.GenAI;
using Google.GenAI.Types;
using Firmness.Application.Interfaces;
using Firmness.Application.DTOs.Excel;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Firmness.Infrastructure.Services.Gemini;

public class GeminiService : IGeminiService
{
    private readonly Client _client;
    private readonly ILogger<GeminiService> _logger;
    private const string MODEL = "gemini-2.5-flash";

    public GeminiService(Client client, ILogger<GeminiService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<ExcelHeadersResponseDto?> CorrectColumnNamesAsync(
        List<string> realColumns,
        List<string> correctColumns)
    {
        try
        {
            var prompt = $@"
            Corrige nombres de columnas de Excel. 
            No inventes columnas nuevas ni cambies el orden.

            Headers válidos:
            {string.Join(", ", correctColumns)}

            Headers reales:
            {string.Join(", ", realColumns)}

            Devuelve estrictamente este JSON:
            {{
            ""CorrectHeaders"": [...],
            ""OriginalHeaders"": [...],
            ""WasCorrected"": true/false,
            ""ChangesReport"": ""texto explicando los cambios""
            }}
            ";

            var response = await _client.Models.GenerateContentAsync(
                model: MODEL,
                contents: prompt
            );

            var text = response.Candidates[0].Content.Parts[0].Text;

            // 1. Limpia marcas ```json, ``` y basura
            text = text.Replace("```json", "")
                    .Replace("```", "")
                    .Trim();

            // 2. Extrae solo el JSON si viene rodeado de texto
            int firstBrace = text.IndexOf('{');
            int lastBrace = text.LastIndexOf('}');

            if (firstBrace >= 0 && lastBrace > firstBrace)
            {
                text = text.Substring(firstBrace, lastBrace - firstBrace + 1);
            }
            else
            {
                _logger.LogError("AI response does not contain valid JSON: {response}", text);
                return null;
            }

            _logger.LogInformation("Sanitized AI JSON: {json}", text);

            // 3. Intentar deserializar
            var dto = JsonSerializer.Deserialize<ExcelHeadersResponseDto>(text);

            if (dto != null)
            {
                // Fallback: Si CorrectedColumns viene vacío pero CorrectHeaders tiene datos,
                // asumimos que CorrectHeaders son las columnas corregidas en orden.
                if ((dto.CorrectedColumns == null || !dto.CorrectedColumns.Any()) && 
                    dto.CorrectHeaders != null && dto.CorrectHeaders.Any())
                {
                    dto.CorrectedColumns = new List<string>(dto.CorrectHeaders);
                }
            }

            return dto;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Gemini AI error");
            return null;
        }
    }
}
