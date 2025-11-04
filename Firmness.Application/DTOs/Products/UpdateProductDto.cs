namespace Firmness.Application.DTOs.Products;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// DTO to update an existing product.
/// Used in: edit form.
/// </summary>
public class UpdateProductDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "The name must be between 3 and 100 characters long")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category is required")]
    [StringLength(50, ErrorMessage = "The category cannot exceed 50 characters")]
    public string Category { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "The description cannot exceed 500 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Code is required")]
    [StringLength(20, ErrorMessage = "The code cannot exceed 20 characters")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "The price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Stock is required")]
    [Range(0, int.MaxValue, ErrorMessage = "The stock cannot be negative")]
    public int Stock { get; set; }
    
    public bool IsActive { get; set; } = true;
}