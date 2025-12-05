using OfficeOpenXml;

namespace Firmness.Application.Common;

public static class EPPlusHelper
{
    // Lee todas las filas usando los headers corregidos como claves
    public static List<Dictionary<string, string>> ReadRows(
        ExcelWorksheet ws,
        List<string> correctedHeaders,
        int startRow = 2)
    {
        var rows = new List<Dictionary<string, string>>();

        int lastRow = ws.Dimension.End.Row;
        int lastCol = correctedHeaders.Count;

        for (int r = startRow; r <= lastRow; r++)
        {
            var rowDict = new Dictionary<string, string>();

            for (int c = 1; c <= lastCol; c++)
            {
                string key = correctedHeaders[c - 1];
                string value = ws.Cells[r, c].Text?.Trim() ?? "";
                rowDict[key] = value;
            }

            rows.Add(rowDict);
        }

        return rows;
    }
}
