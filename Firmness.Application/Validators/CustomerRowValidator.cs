namespace Firmness.Application.Validators;

using Firmness.Application.DTOs.Excel;
using Firmness.Application.Interfaces;

public class CustomerRowValidator : IExcelRowValidator
{
    public RowValidationResultDto Validate(Dictionary<string, string> row, int rowNumber)
    {
        var result = new RowValidationResultDto
        {
            RowNumber = rowNumber,
            RowData = row
        };

        // Validación de campos obligatorios
        string[] required = { "Username", "FullName", "Email" };

        foreach (var field in required)
        {
            if (!row.ContainsKey(field) || string.IsNullOrWhiteSpace(row[field]))
                result.Errors.Add($"El campo '{field}' es obligatorio.");
        }

        // Validación de email
        if (row.TryGetValue("Email", out var email))
        {
            if (!email.Contains("@"))
                result.Errors.Add("El email no tiene un formato válido.");
        }

        result.IsValid = result.Errors.Count == 0;
        return result;
    }
}
