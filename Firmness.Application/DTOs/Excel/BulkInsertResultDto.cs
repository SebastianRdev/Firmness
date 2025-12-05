namespace Firmness.Application.DTOs.Excel;

public class BulkInsertResultDto
{
    public int InsertedCount { get; set; }
    public List<int> FailedRows { get; set; } = new();
}
