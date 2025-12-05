namespace Firmness.Application.DTOs.Excel;

/// <summary>
/// Data transfer object for the response of Excel header extraction and correction.
/// </summary>
public class ExcelHeadersResponseDto
{
    /// <summary>
    /// Gets or sets the original headers found in the Excel file.
    /// </summary>
    public List<string> OriginalHeaders { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the correct headers expected by the system.
    /// </summary>
    public List<string> CorrectHeaders { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the corrected column names mapped to the original headers.
    /// </summary>
    public List<string> CorrectedColumns { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets a value indicating whether corrections were made.
    /// </summary>
    public bool WasCorrected { get; set; }

    /// <summary>
    /// Gets or sets a report detailing the changes made during correction.
    /// </summary>
    public string ChangesReport { get; set; } = string.Empty;
}