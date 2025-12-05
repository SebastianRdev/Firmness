using Firmness.Application.DTOs.Excel;
using Firmness.Application.Common; // For ResultOft<T>
using Microsoft.AspNetCore.Http;

namespace Firmness.Application.Interfaces;

public interface IImportService
{
    /// <summary>
    /// Processes an uploaded Excel file to generate a preview of the import.
    /// </summary>
    /// <param name="file">The uploaded Excel file.</param>
    /// <param name="entityType">The type of entity to import.</param>
    /// <returns>A result containing the preview data.</returns>
    Task<ResultOft<BulkPreviewResultDto>> ProcessExcelPreviewAsync(IFormFile file, string entityType);

    /// <summary>
    /// Confirms the import operation based on the preview data.
    /// </summary>
    /// <param name="previewModel">The preview model containing valid rows to insert.</param>
    /// <returns>A result containing the outcome of the bulk insert.</returns>
    Task<ResultOft<BulkInsertResultDto>> ConfirmImportAsync(BulkPreviewResultDto previewModel);
}
