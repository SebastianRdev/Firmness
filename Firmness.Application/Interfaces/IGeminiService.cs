namespace Firmness.Application.Interfaces;

using Firmness.Application.DTOs.Excel;

/// <summary>
/// Service for AI-powered column name correction using Gemini API
/// </summary>
public interface IGeminiService
{
    /// <summary>
    /// Corrects column names using AI to match expected template
    /// </summary>
    /// <param name="realColumns">Actual column names from Excel</param>
    /// <param name="correctColumns">Expected column names from template</param>
    /// <returns>Correction result with mapped columns and change report</returns>
    Task<ExcelHeadersResponseDto?> CorrectColumnNamesAsync(
        List<string> realColumns,
        List<string> correctColumns);
}