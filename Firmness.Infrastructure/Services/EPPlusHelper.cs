using OfficeOpenXml;

namespace Firmness.Infrastructure.Services;

public static class EPPlusHelper
{
    public static List<string> ReadHeaders(ExcelWorksheet sheet)
    {
        var headers = new List<string>();
        int colCount = sheet.Dimension.Columns;

        for (int col = 1; col <= colCount; col++)
        {
            var value = sheet.Cells[1, col].Text?.Trim();
            if (!string.IsNullOrWhiteSpace(value))
                headers.Add(value);
        }

        return headers;
    }

    public static List<Dictionary<string, string>> ReadRows(
        ExcelWorksheet sheet,
        List<string> correctedHeaders)
    {
        var rows = new List<Dictionary<string, string>>();
        int rowCount = sheet.Dimension.Rows;
        int colCount = correctedHeaders.Count;

        for (int row = 2; row <= rowCount; row++)
        {
            var rowDict = new Dictionary<string, string>();

            for (int col = 1; col <= colCount; col++)
            {
                var key = correctedHeaders[col - 1];
                var value = sheet.Cells[row, col].Text?.Trim() ?? "";
                rowDict[key] = value;
            }

            rows.Add(rowDict);
        }

        return rows;
    }
}
