namespace Firmness.Application.Interfaces;

using Firmness.Application.DTOs.Excel;

public interface IExcelRowValidator
{
    RowValidationResultDto Validate(
        Dictionary<string, string> row,
        int rowNumber
    );
}

