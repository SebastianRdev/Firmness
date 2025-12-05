namespace Firmness.Application.Interfaces;

using Firmness.Application.DTOs.Excel;

public interface IExcelRowValidator
{
    /// <summary>
    /// Validates a single row from an Excel file.
    /// </summary>
    /// <param name="row">The dictionary representing the row data.</param>
    /// <param name="rowNumber">The row number in the Excel file.</param>
    /// <returns>A result indicating whether the row is valid or contains errors.</returns>
    RowValidationResultDto Validate(
        Dictionary<string, string> row,
        int rowNumber
    );
}

