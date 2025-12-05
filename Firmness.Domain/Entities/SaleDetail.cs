namespace Firmness.Domain.Entities;

/// <summary>
/// Represents a line item in a sale transaction.
/// </summary>
public class SaleDetail
{
    /// <summary>
    /// Gets or sets the unique identifier for the sale detail.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the foreign key for the associated sale.
    /// </summary>
    public int SaleId { get; set; }

    /// <summary>
    /// Gets or sets the sale associated with this detail.
    /// </summary>
    public Sale Sale { get; set; } 

    /// <summary>
    /// Gets or sets the foreign key for the product sold.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the product associated with this detail.
    /// </summary>
    public Product Product { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the product sold.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product at the time of sale.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets the total price for this line item (Quantity * UnitPrice).
    /// </summary>
    public decimal TotalPrice => Quantity * UnitPrice;
}
