namespace Firmness.WebAdmin.Models.Products;

using Microsoft.AspNetCore.Mvc.Rendering;

public class EditProductViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public IEnumerable<SelectListItem>? Categories { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    
    public bool IsActive { get; set; } = true;
}
