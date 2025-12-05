using Firmness.Application.DTOs.Excel;
using Firmness.Application.Interfaces;

namespace Firmness.Application.Validators;

public class ProductRowValidator : IExcelRowValidator
{
    public RowValidationResultDto Validate(Dictionary<string, string> row, int rowNumber)
    {
        var result = new RowValidationResultDto
        {
            RowNumber = rowNumber,
            RowData = row
        };

        string[] required = { "Name", "Category", "Price" };

        foreach (var field in required)
        {
            if (!row.ContainsKey(field) || string.IsNullOrWhiteSpace(row[field]))
                result.Errors.Add($"El campo '{field}' es obligatorio.");
        }

        if (row.TryGetValue("Price", out var priceStr))
        {
            if (!decimal.TryParse(priceStr, out _))
                result.Errors.Add("El precio no es un valor numérico válido.");
        }

        result.IsValid = result.Errors.Count == 0;
        return result;
    }
}
