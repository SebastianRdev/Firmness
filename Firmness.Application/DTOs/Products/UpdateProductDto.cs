namespace Firmness.Application.DTOs.Products;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Data transfer object for updating an existing product.
/// </summary>
public class UpdateProductDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the product.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "The name must be between 3 and 100 characters long")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category identifier.
    /// </summary>
    [Required(ErrorMessage = "Category is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a category")]
    public int CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the product description.
    /// </summary>
    [StringLength(500, ErrorMessage = "The description cannot exceed 500 characters")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product code or SKU.
    /// </summary>
    [Required(ErrorMessage = "Code is required")]
    [StringLength(20, ErrorMessage = "The code cannot exceed 20 characters")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product price.
    /// </summary>
    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "The price must be greater than 0")]
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the product stock quantity.
    /// </summary>
    [Required(ErrorMessage = "Stock is required")]
    [Range(0, int.MaxValue, ErrorMessage = "The stock cannot be negative")]
    public int Stock { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the product is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}