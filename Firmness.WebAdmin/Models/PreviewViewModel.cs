namespace Firmness.WebAdmin.Models;

/// <summary>
/// ViewModel for previewing bulk import data.
/// </summary>
public class PreviewViewModel
{
    /// <summary>
    /// Gets or sets the type of entity being imported.
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of corrected headers.
    /// </summary>
    public List<string> CorrectedHeaders { get; set; } = new();

    /// <summary>
    /// Gets or sets the rows of data to be imported.
    /// </summary>
    public List<Dictionary<string, string>> Rows { get; set; } = new();
}
