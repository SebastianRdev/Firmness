namespace Firmness.Application.Interfaces;

using Microsoft.AspNetCore.Http;

public interface IExcelService
{
    Task<List<string>> GetHeadersAsync(IFormFile file);
    Task<List<Dictionary<string, string>>> ReadRowsAsync(IFormFile file, int startRow = 2);
}