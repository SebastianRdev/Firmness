namespace Firmness.Application.DTOs.Sales;

/// <summary>
/// Data transfer object for sale response details.
/// </summary>
public class SaleResponseDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the sale.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the customer.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the name of the customer.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email of the customer.
    /// </summary>
    public string CustomerEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time of the sale.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the total amount before taxes.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the tax amount.
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// Gets or sets the grand total amount including taxes.
    /// </summary>
    public decimal GrandTotal { get; set; }

    /// <summary>
    /// Gets or sets the filename of the receipt, if available.
    /// </summary>
    public string? ReceiptFileName { get; set; }

    /// <summary>
    /// Gets or sets the list of sale details (line items).
    /// </summary>
    public List<SaleDetailDto> SaleDetails { get; set; } = new();
}

/// <summary>
/// Data transfer object for sale detail response.
/// </summary>
public class SaleDetailDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the sale detail.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity of the product.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets the total price for this line item.
    /// </summary>
    public decimal Total => Quantity * UnitPrice;
}
