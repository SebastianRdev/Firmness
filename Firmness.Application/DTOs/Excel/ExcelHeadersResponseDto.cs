namespace Firmness.Application.DTOs.Excel;

public class ExcelHeadersResponseDto
{
    public List<string> OriginalHeaders { get; set; } = new();
    public bool HasHeaders => OriginalHeaders != null && OriginalHeaders.Count > 0;
}