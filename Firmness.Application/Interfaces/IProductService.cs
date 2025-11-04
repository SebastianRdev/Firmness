namespace Firmness.Application.Interfaces;

using Firmness.Application.DTOs.Products;
using Firmness.Application.Common;

public interface IProductService
{
    /// <summary>
    /// It obtains all the products of the system.
    /// </summary>
    Task<ResultOft<IEnumerable<ProductDto>>> GetAllAsync();

    /// <summary>
    /// You get a product by your ID.
    /// </summary>
    Task<ResultOft<ProductDto>> GetByIdAsync(int id);

    /// <summary>
    /// Create a new product.
    /// </summary>
    Task<ResultOft<ProductDto>> CreateAsync(CreateProductDto createDto);

    /// <summary>
    /// Update an existing product.
    /// </summary>
    Task<ResultOft<ProductDto>> UpdateAsync(UpdateProductDto updateDto);

    /// <summary>
    /// Delete a product.
    /// </summary>
    Task<Result> DeleteAsync(int id);

    /// <summary>
    /// Search for products by term.
    /// </summary>
    Task<ResultOft<IEnumerable<ProductDto>>> SearchAsync(string searchTerm);

    /// <summary>
    /// Check if a product exists.
    /// </summary>
    Task<bool> ExistsAsync(int id);
}