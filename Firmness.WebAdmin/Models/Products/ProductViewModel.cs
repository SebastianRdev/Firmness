namespace Firmness.WebAdmin.Models.Products;

/// <summary>
/// ViewModel for displaying product details.
/// </summary>
public class ProductViewModel
{
    /// <summary>
    /// Gets or sets the product ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category ID.
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the category name.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product price.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the stock quantity.
    /// </summary>
    public int Stock { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the product is active.
    /// </summary>
    public bool IsActive { get; set; }
    
    // Calculated property to display in the view
    /// <summary>
    /// Gets the formatted price string.
    /// </summary>
    public string PriceFormatted => $"${Price:N0} COP";

    /// <summary>
    /// Gets the stock status string.
    /// </summary>
    public string StockStatus => Stock > 0 ? "Available": "Out of stock";
}