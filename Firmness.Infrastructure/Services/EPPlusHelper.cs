using OfficeOpenXml;

namespace Firmness.Infrastructure.Services;

/// <summary>
/// Helper class for EPPlus Excel operations.
/// </summary>
public static class EPPlusHelper
{
    /// <summary>
    /// Reads headers from the first row of an Excel worksheet.
    /// </summary>
    /// <param name="sheet">The Excel worksheet.</param>
    /// <returns>A list of header names.</returns>
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

    /// <summary>
    /// Reads rows from an Excel worksheet using corrected headers.
    /// </summary>
    /// <param name="sheet">The Excel worksheet.</param>
    /// <param name="correctedHeaders">The list of corrected header names.</param>
    /// <returns>A list of dictionaries representing the rows.</returns>
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
