using Firmness.Application.Common;
using Firmness.Application.DTOs.Excel;
using Microsoft.AspNetCore.Http;

namespace Firmness.Application.Interfaces;

/// <summary>
/// Service interface for Customer Excel import operations
/// </summary>
public interface ICustomerImportService
{
    /// <summary>
    /// Imports customers from an Excel file
    /// </summary>
    Task<Result> ImportFromExcelAsync(IFormFile file, string entityType);
    
    /// <summary>
    /// Extracts headers from an Excel file for preview
    /// </summary>
    Task<ResultOft<ExcelHeadersResponseDto>> ExtractHeadersFromExcelAsync(IFormFile file);
}
