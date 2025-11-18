namespace Firmness.Application.DTOs.Categories;

using Firmness.Application.DTOs.Categories;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public IEnumerable<CategoryDto>? Categories { get; set; }
}
