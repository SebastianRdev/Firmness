namespace Firmness.WebAdmin.Models.Products;

using Microsoft.AspNetCore.Mvc.Rendering;

/// <summary>
/// ViewModel for editing an existing product.
/// </summary>
public class EditProductViewModel
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
    /// Gets or sets the list of available categories.
    /// </summary>
    public IEnumerable<SelectListItem>? Categories { get; set; }

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
    public bool IsActive { get; set; } = true;
}
