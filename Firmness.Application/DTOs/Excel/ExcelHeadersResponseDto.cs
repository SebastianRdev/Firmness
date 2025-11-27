namespace Firmness.Application.DTOs.Excel;

public class ExcelHeadersResponseDto
{
    public List<string> OriginalHeaders { get; set; } = new List<string>();
    public List<string> CorrectHeaders { get; set; } = new List<string>();
    public List<string> CorrectedColumns { get; set; } = new List<string>();
    public bool WasCorrected { get; set; }
    public string ChangesReport { get; set; } = string.Empty;
}