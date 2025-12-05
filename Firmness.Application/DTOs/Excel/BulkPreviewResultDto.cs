namespace Firmness.Application.DTOs.Excel;

/// <summary>
/// Data transfer object for the preview of a bulk insert operation.
/// </summary>
public class BulkPreviewResultDto
{
    /// <summary>
    /// Gets or sets the list of valid rows ready for insertion.
    /// </summary>
    public List<RowValidationResultDto> ValidRows { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of invalid rows with errors.
    /// </summary>
    public List<RowValidationResultDto> InvalidRows { get; set; } = new();

    /// <summary>
    /// Gets the total number of rows processed.
    /// </summary>
    public int TotalRows => ValidRows.Count + InvalidRows.Count;

    /// <summary>
    /// Gets the total number of valid rows.
    /// </summary>
    public int TotalValid => ValidRows.Count;

    /// <summary>
    /// Gets the total number of invalid rows.
    /// </summary>
    public int TotalInvalid => InvalidRows.Count;
}
