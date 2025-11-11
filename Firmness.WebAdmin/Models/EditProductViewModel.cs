namespace Firmness.WebAdmin.Models;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class EditProductViewModel
{
    [HiddenInput]
    public int Id { get; set; }

    [Required(ErrorMessage = "The name is mandatory")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "The price must be greater than 0")]
    public decimal Price { get; set; }

    [Display(Name = "Category")]
    [Required(ErrorMessage = "Select a category")]
    public int CategoryId { get; set; }

    public IEnumerable<SelectListItem>? Categories { get; set; }
}
