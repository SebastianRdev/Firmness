namespace Firmness.Application.Interfaces;

using Microsoft.AspNetCore.Http;
using Firmness.Application.Common;
using Firmness.Application.DTOs.Excel;

public interface IExcelService
{
    Task<List<string>> GetHeadersAsync(IFormFile file);

    Task<ResultOft<ExcelHeadersResponseDto>> ExtractHeadersFromExcelAsync(
        IFormFile file,
        string entityType);
    Task<List<Dictionary<string, string>>> ReadRowsAsync(IFormFile file, int startRow = 2);

    Task<ResultOft<ExcelHeadersResponseDto>> CorrectColumnNamesAsync(List<string> realColumns,
        List<string> correctColumns);

    Task<BulkPreviewResultDto> GeneratePreviewAsync(
        Stream fileStream,
        string entityType,
        List<string> correctedHeaders);
}