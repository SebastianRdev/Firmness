namespace Firmness.Application.Services;

using Firmness.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

public class ExcelService : IExcelService
{
    public async Task<List<string>> GetHeadersAsync(IFormFile file)
    {
        try
        {
            using var stream = file.OpenReadStream();
            using var package = new ExcelPackage(stream);
            var ws = package.Workbook.Worksheets.First();

            int colCount = ws.Dimension?.End.Column ?? 0;
            int row = 1;

            var headers = new List<string>();

            for (int c = 1; c <= colCount; c++)
            {
                var header = ws.Cells[row, c].Text?.Trim() ?? string.Empty;
                headers.Add(header);
            }

            return headers;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ExcelService] Error leyendo headers: {ex.Message}");
            throw; // para que el logger del servicio también lo capture
        }
    }


    public async Task<List<Dictionary<string, string>>> ReadRowsAsync(IFormFile file, int startRow = 2)
    {
        using var stream = file.OpenReadStream();
        using var package = new ExcelPackage(stream);
        var ws = package.Workbook.Worksheets.First();

        int rows = ws.Dimension.End.Row;
        int cols = ws.Dimension.End.Column;

        var list = new List<Dictionary<string, string>>();

        for (int r = startRow; r <= rows; r++)
        {
            var dict = new Dictionary<string, string>();
            for (int c = 1; c <= cols; c++)
            {
                dict[$"col{c}"] = ws.Cells[r, c].Text?.Trim() ?? "";
            }
            list.Add(dict);
        }

        return list;
    }
}