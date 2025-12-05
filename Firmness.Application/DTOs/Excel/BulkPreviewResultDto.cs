namespace Firmness.Application.DTOs.Excel;

public class BulkPreviewResultDto
{
    public List<RowValidationResultDto> ValidRows { get; set; } = new();
    public List<RowValidationResultDto> InvalidRows { get; set; } = new();

    public int TotalRows => ValidRows.Count + InvalidRows.Count;
    public int TotalValid => ValidRows.Count;
    public int TotalInvalid => InvalidRows.Count;
}
