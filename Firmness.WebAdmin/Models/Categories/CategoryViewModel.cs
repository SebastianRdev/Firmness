namespace Firmness.WebAdmin.Models.Categories;

using Firmness.Application.DTOs.Categories;

public class CategoryViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public IEnumerable<CategoryDto>? Categories { get; set; }
}