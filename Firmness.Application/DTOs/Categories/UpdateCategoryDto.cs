namespace Firmness.Application.DTOs.Categories;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Data transfer object for updating an existing category.
/// </summary>
public class UpdateCategoryDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the category.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the category.
    /// </summary>
    [Required(ErrorMessage = "The name is required")]
    [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;
}