namespace Firmness.Application.DTOs.Sales;

/// <summary>
/// Data transfer object for creating a new sale.
/// </summary>
public class CreateSaleDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the customer.
    /// </summary>
    public Guid CustomerId { get; set; }

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
    /// Gets or sets the list of sale details (line items).
    /// </summary>
    public List<CreateSaleDetailDto> SaleDetails { get; set; } = new();
}

/// <summary>
/// Data transfer object for creating a sale detail (line item).
/// </summary>
public class CreateSaleDetailDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the product.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the product.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product.
    /// </summary>
    public decimal UnitPrice { get; set; }
}
