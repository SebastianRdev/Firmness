namespace Firmness.Application.DTOs.Categories;

using System.ComponentModel.DataAnnotations;

public class UpdateCategoryDto
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "The name is required")]
    [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;
}