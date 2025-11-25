namespace Firmness.WebAdmin.Models.Products;

using Microsoft.AspNetCore.Mvc.Rendering;

public class CreateProductViewModel
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }

    public IEnumerable<SelectListItem>? Categories { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Stock { get; set; }
}
