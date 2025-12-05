namespace Firmness.Application.Interfaces;

using Firmness.Application.DTOs.Excel;

/// <summary>
/// Service for AI-powered column name correction using Gemini API.
/// </summary>
public interface IGeminiService
{
    /// <summary>
    /// Corrects column names using AI to match the expected template.
    /// </summary>
    /// <param name="realColumns">Actual column names found in the Excel file.</param>
    /// <param name="correctColumns">Expected column names from the template.</param>
    /// <returns>A correction result with mapped columns and a change report, or null if correction fails.</returns>
    Task<ExcelHeadersResponseDto?> CorrectColumnNamesAsync(
        List<string> realColumns,
        List<string> correctColumns);
}