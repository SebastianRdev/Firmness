namespace Firmness.Infrastructure.Services.Gemini;

using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Firmness.Application.Interfaces;
using Firmness.Application.DTOs.Excel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class GeminiApiClient : IGeminiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<GeminiApiClient> _logger;

    public GeminiApiClient(
        IConfiguration config, 
        HttpClient http,
        ILogger<GeminiApiClient> logger)
    {
        _httpClient = http;
        // Lee desde variable de entorno (.env)
        _apiKey = config["GEMINI_API_KEY"] 
            ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY")
            ?? throw new ArgumentNullException("GEMINI_API_KEY", 
                "GEMINI_API_KEY not found in configuration or environment variables");
        _logger = logger;
    }

    public async Task<ExcelHeadersResponseDto?> CorrectColumnNamesAsync(
        List<string> realColumns,
        List<string> correctColumns)
    {
        try
        {
            var prompt = GeneratePrompt(realColumns, correctColumns);
            var body = await PostRequestAsync(prompt);
            return ProcessResponse(body, realColumns, correctColumns);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Gemini AI for column correction");
            return null;
        }
    }

    private async Task<string> PostRequestAsync(string prompt)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-exp:generateContent?key={_apiKey}";

        var payload = new
        {
            contents = new[]
            {
                new {
                    parts = new[] {
                        new { text = prompt }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 0.3,
                maxOutputTokens = 2048
            }
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _logger.LogInformation("Calling Gemini API for column correction");
        
        var response = await _httpClient.PostAsync(url, content);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Gemini API error: {StatusCode} - {Body}", response.StatusCode, body);
            throw new Exception($"Gemini API error: {response.StatusCode}");
        }

        return body;
    }

    private ExcelHeadersResponseDto? ProcessResponse(
        string body, 
        List<string> originalHeaders,
        List<string> correctHeaders)
    {
        try
        {
            var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(body);

            if (geminiResponse?.Candidates?.Any() != true)
            {
                _logger.LogWarning("No candidates returned from Gemini");
                return null;
            }

            var firstCandidate = geminiResponse.Candidates.First();
            var text = firstCandidate?.Content?.Parts?.FirstOrDefault()?.Text;

            if (string.IsNullOrWhiteSpace(text))
            {
                _logger.LogWarning("Empty text returned from Gemini");
                return null;
            }

            // Limpiar markdown si viene envuelto en ```json
            text = text.Trim();
            if (text.StartsWith("```json"))
                text = text.Substring(7);
            if (text.StartsWith("```"))
                text = text.Substring(3);
            if (text.EndsWith("```"))
                text = text.Substring(0, text.Length - 3);
            text = text.Trim();

            var options = new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            };
            
            var correctionResult = JsonSerializer.Deserialize<CorrectionResult>(text, options);

            if (correctionResult == null)
            {
                _logger.LogWarning("Failed to deserialize Gemini response");
                return null;
            }

            return new ExcelHeadersResponseDto
            {
                OriginalHeaders = originalHeaders,
                CorrectHeaders = correctHeaders,
                CorrectedColumns = correctionResult.CorrectedColumns ?? originalHeaders,
                WasCorrected = correctionResult.WasCorrected,
                ChangesReport = correctionResult.ChangesReport ?? "No changes needed"
            };
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse Gemini response: {Body}", body);
            return null;
        }
    }

    private string GeneratePrompt(List<string> realColumns, List<string> correctColumns)
    {
        return $@"You are a column name correction assistant for Excel imports.

        **Task**: Match the real column names to the correct expected names.

        **Real columns from Excel**: {string.Join(", ", realColumns)}

        **Expected correct columns**: {string.Join(", ", correctColumns)}

        **Instructions**:
        1. Map each real column to the most similar correct column
        2. If a real column closely matches a correct one (ignoring case, spaces, accents), map them
        3. If no match exists, keep the original name

        **Return ONLY valid JSON** with this structure (no markdown, no explanation):
        {{
          ""correctedColumns"": [""mapped_name1"", ""mapped_name2"", ...],
          ""wasCorrected"": true/false,
          ""changesReport"": ""Brief description of changes made or 'No changes needed'""
        }}

The correctedColumns array must have exactly {realColumns.Count} elements in the same order as the real columns.";
    }

    // Response models for Gemini API
    private class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public List<Candidate>? Candidates { get; set; }
    }

    private class Candidate
    {
        [JsonPropertyName("content")]
        public Content? Content { get; set; }
    }

    private class Content
    {
        [JsonPropertyName("parts")]
        public List<Part>? Parts { get; set; }
    }

    private class Part
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }

    private class CorrectionResult
    {
        [JsonPropertyName("correctedColumns")]
        public List<string>? CorrectedColumns { get; set; }

        [JsonPropertyName("wasCorrected")]
        public bool WasCorrected { get; set; }

        [JsonPropertyName("changesReport")]
        public string? ChangesReport { get; set; }
    }
}