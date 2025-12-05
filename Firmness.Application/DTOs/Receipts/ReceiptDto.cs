namespace Firmness.Application.DTOs.Receipts;

/// <summary>
/// Data transfer object for receipt details.
/// </summary>
public class ReceiptDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the receipt.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the foreign key for the associated sale.
    /// </summary>
    public int SaleId { get; set; }

    /// <summary>
    /// Gets or sets the human-readable receipt number.
    /// </summary>
    public string ReceiptNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the receipt was issued.
    /// </summary>
    public DateTime IssueDate { get; set; }

    /// <summary>
    /// Gets or sets the name of the PDF receipt file.
    /// </summary>
    public string FileName { get; set; } = string.Empty;
}
