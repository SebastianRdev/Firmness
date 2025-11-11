namespace Firmness.WebAdmin.Models;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

public class CreateProductViewModel
{
    [Required(ErrorMessage = "The name is mandatory")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "The price must be greater than 0")]
    public decimal Price { get; set; }

    [Display(Name = "Category")]
    [Required(ErrorMessage = "Select a category")]
    public int CategoryId { get; set; }

    public IEnumerable<SelectListItem>? Categories { get; set; }
    
    [Required(ErrorMessage = "Code is required")]
    [StringLength(20, ErrorMessage = "The code cannot exceed 20 characters")]
    public string Code { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "The description is mandatory")]
    [StringLength(100)]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Stock is required")]
    [Range(0, int.MaxValue, ErrorMessage = "The stock cannot be negative")]
    public int Stock { get; set; }
}
