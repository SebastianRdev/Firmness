namespace Firmness.Application.DTOs.Excel;

/// <summary>
/// Data transfer object for the result of validating a single Excel row.
/// </summary>
public class RowValidationResultDto
{
    /// <summary>
    /// Gets or sets the row number in the Excel file.
    /// </summary>
    public int RowNumber { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the row is valid.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Gets or sets the data contained in the row as key-value pairs.
    /// </summary>
    public Dictionary<string, string> RowData { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of validation errors for the row.
    /// </summary>
    public List<string> Errors { get; set; } = new();
}
