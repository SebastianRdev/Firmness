namespace Firmness.Application.DTOs.Products;

/// <summary>
/// DTO para mostrar información de un producto.
/// Usado en: listados, detalles, respuestas de API.
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
    
    // Propiedad calculada para mostrar en la vista
    public string PriceFormatted => $"${Price:N0} COP";
    public string StockStatus => Stock > 0 ? "Disponible" : "Agotado";
}