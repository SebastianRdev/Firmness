namespace Firmness.Application.Interfaces;

using Microsoft.AspNetCore.Http;
using Firmness.Application.Common;
using Firmness.Application.DTOs.Excel;

public interface IExcelService
{
    /// <summary>
    /// Retrieves the headers from an uploaded Excel file.
    /// </summary>
    /// <param name="file">The uploaded Excel file.</param>
    /// <returns>A list of header names found in the file.</returns>
    Task<List<string>> GetHeadersAsync(IFormFile file);

    /// <summary>
    /// Extracts headers from an Excel file and validates them against the entity type.
    /// </summary>
    /// <param name="file">The uploaded Excel file.</param>
    /// <param name="entityType">The type of entity to validate headers against.</param>
    /// <returns>A result containing the extracted and potentially corrected headers.</returns>
    Task<ResultOft<ExcelHeadersResponseDto>> ExtractHeadersFromExcelAsync(
        IFormFile file,
        string entityType);

    /// <summary>
    /// Reads rows from an Excel file starting from a specified row.
    /// </summary>
    /// <param name="file">The uploaded Excel file.</param>
    /// <param name="startRow">The row number to start reading from (default is 2).</param>
    /// <returns>A list of dictionaries representing the rows, where keys are column names.</returns>
    Task<List<Dictionary<string, string>>> ReadRowsAsync(IFormFile file, int startRow = 2);

    /// <summary>
    /// Corrects column names based on a list of expected correct columns.
    /// </summary>
    /// <param name="realColumns">The actual columns found in the file.</param>
    /// <param name="correctColumns">The expected correct column names.</param>
    /// <returns>A result containing the corrected headers.</returns>
    Task<ResultOft<ExcelHeadersResponseDto>> CorrectColumnNamesAsync(List<string> realColumns,
        List<string> correctColumns);

    /// <summary>
    /// Generates a preview of the bulk insert operation.
    /// </summary>
    /// <param name="fileStream">The stream of the Excel file.</param>
    /// <param name="entityType">The type of entity being imported.</param>
    /// <param name="correctedHeaders">The list of corrected headers to use for mapping.</param>
    /// <returns>A result containing the preview of valid and invalid rows.</returns>
    Task<BulkPreviewResultDto> GeneratePreviewAsync(
        Stream fileStream,
        string entityType,
        List<string> correctedHeaders);
}