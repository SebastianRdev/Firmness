namespace Firmness.Application.DTOs.Categories;

using Firmness.Application.DTOs.Products;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public IEnumerable<ProductDto>? Products { get; set; }
}
