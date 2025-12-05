namespace Firmness.WebAdmin.Models;

public class PreviewViewModel
{
    public string EntityType { get; set; } = string.Empty;
    public List<string> CorrectedHeaders { get; set; } = new();
    public List<Dictionary<string, string>> Rows { get; set; } = new();
}
