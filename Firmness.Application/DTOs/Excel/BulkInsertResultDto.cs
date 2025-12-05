namespace Firmness.Application.DTOs.Excel;

/// <summary>
/// Data transfer object for the result of a bulk insert operation.
/// </summary>
public class BulkInsertResultDto
{
    /// <summary>
    /// Gets or sets the number of successfully inserted rows.
    /// </summary>
    public int InsertedCount { get; set; }

    /// <summary>
    /// Gets or sets the list of row numbers that failed to insert.
    /// </summary>
    public List<int> FailedRows { get; set; } = new();
}
