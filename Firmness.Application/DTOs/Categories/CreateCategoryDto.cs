namespace Firmness.Application.DTOs.Categories;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Data transfer object for creating a new category.
/// </summary>
public class CreateCategoryDto
{
    /// <summary>
    /// Gets or sets the name of the category.
    /// </summary>
    [Required(ErrorMessage = "The name is required")]
    [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;
}