namespace Firmness.Domain.Entities;

/// <summary>
/// Represents a receipt generated for a sale.
/// </summary>
public class Receipt
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
    public string ReceiptNumber { get; set; }

    /// <summary>
    /// Gets or sets the sale associated with the receipt.
    /// </summary>
    public Sale Sale { get; set; }

    /// <summary>
    /// Gets or sets the name of the PDF receipt file.
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Gets or sets the full path where the PDF receipt is stored.
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the receipt was generated.
    /// </summary>
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}
