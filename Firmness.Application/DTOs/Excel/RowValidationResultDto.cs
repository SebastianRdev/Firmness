namespace Firmness.Application.DTOs.Excel;

public class RowValidationResultDto
{
    public int RowNumber { get; set; }
    public bool IsValid { get; set; }
    public Dictionary<string, string> RowData { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}
