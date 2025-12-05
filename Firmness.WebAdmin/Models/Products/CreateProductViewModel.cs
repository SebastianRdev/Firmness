namespace Firmness.WebAdmin.Models.Products;

using Microsoft.AspNetCore.Mvc.Rendering;

/// <summary>
/// ViewModel for creating a new product.
/// </summary>
public class CreateProductViewModel
{
    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product price.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the category ID.
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the list of available categories.
    /// </summary>
    public IEnumerable<SelectListItem>? Categories { get; set; }

    /// <summary>
    /// Gets or sets the product code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the stock quantity.
    /// </summary>
    public int Stock { get; set; }
}
