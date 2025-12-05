using Firmness.Application.DTOs.Excel;
using Firmness.Application.Common; // For ResultOft<T>
using Microsoft.AspNetCore.Http;

namespace Firmness.Application.Interfaces;

public interface IImportService
{
    Task<ResultOft<BulkPreviewResultDto>> ProcessExcelPreviewAsync(IFormFile file, string entityType);
    Task<ResultOft<BulkInsertResultDto>> ConfirmImportAsync(BulkPreviewResultDto previewModel);
}
