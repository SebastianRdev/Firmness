namespace Firmness.Application.DTOs.Products;

/// <summary>
/// DTO to display product information.
/// Used in: listings, details, API responses.
/// </summary>
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; }
    
    // Calculated property to display in the view
    public string PriceFormatted => $"${Price:N0} COP";
    public string StockStatus => Stock > 0 ? "Available": "Out of stock";
}