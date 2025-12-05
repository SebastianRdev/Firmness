namespace Firmness.Domain.Entities;

/// <summary>
/// Represents a sale transaction.
/// </summary>
public class Sale
{
    /// <summary>
    /// Gets or sets the unique identifier for the sale.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the date and time of the sale.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the foreign key for the customer who made the purchase.
    /// </summary>
    public string CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the customer associated with the sale.
    /// </summary>
    public Customer Customer { get; set; }

    /// <summary>
    /// Gets or sets the total amount of the sale before taxes and fees.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the tax amount for the sale.
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// Gets or sets the delivery fees for the sale.
    /// </summary>
    public decimal DeliveryFees { get; set; }

    /// <summary>
    /// Gets or sets the grand total amount including taxes and fees.
    /// </summary>
    public decimal GrandTotal { get; set; }

    /// <summary>
    /// Gets or sets the filename of the generated receipt.
    /// </summary>
    public string ReceiptFileName { get; set; }

    /// <summary>
    /// Gets or sets the collection of sale details (line items).
    /// </summary>
    public ICollection<SaleDetail> SaleDetails { get; set; }

    /// <summary>
    /// Gets or sets the receipt associated with the sale.
    /// </summary>
    public Receipt Receipt { get; set; }
}
