namespace Firmness.Application.Interfaces;

using Firmness.Application.DTOs.Products;
using Firmness.Application.Common;

public interface ICategoryService
{
    /// <summary>
    /// It obtains all the categories of the system.
    /// </summary>
    Task<ResultOft<IEnumerable<ProductDto>>> GetAllAsync();

    /// <summary>
    /// You get a category by your ID.
    /// </summary>
    Task<ResultOft<ProductDto>> GetByIdAsync(int id);

    /// <summary>
    /// Create a new category.
    /// </summary>
    Task<ResultOft<ProductDto>> CreateAsync(CreateProductDto createDto);

    /// <summary>
    /// Update an existing category.
    /// </summary>
    Task<ResultOft<ProductDto>> UpdateAsync(UpdateProductDto updateDto);

    /// <summary>
    /// Delete a product.
    /// </summary>
    Task<Result> DeleteAsync(int id);

    /// <summary>
    /// Search for categories by term.
    /// </summary>
    Task<ResultOft<IEnumerable<ProductDto>>> SearchAsync(string searchTerm);

    /// <summary>
    /// Check if a category exists.
    /// </summary>
    Task<bool> ExistsAsync(int id);
}