namespace Firmness.Application.Interfaces;

using Firmness.Application.DTOs.Products;
using Firmness.Application.Common;

public interface IProductService
{
    /// <summary>
    /// Retrieves all products in the system.
    /// </summary>
    /// <returns>A result containing a collection of product DTOs.</returns>
    Task<ResultOft<IEnumerable<ProductDto>>> GetAllAsync();

    /// <summary>
    /// Retrieves a product by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <returns>A result containing the product DTO if found.</returns>
    Task<ResultOft<ProductDto>> GetByIdAsync(int id);

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="createDto">The data transfer object containing product creation details.</param>
    /// <returns>A result containing the created product DTO.</returns>
    Task<ResultOft<ProductDto>> CreateAsync(CreateProductDto createDto);

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="updateDto">The data transfer object containing product update details.</param>
    /// <returns>A result containing the updated product DTO.</returns>
    Task<ResultOft<ProductDto>> UpdateAsync(UpdateProductDto updateDto);

    /// <summary>
    /// Deletes a product by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product to delete.</param>
    /// <returns>A result indicating the success or failure of the operation.</returns>
    Task<Result> DeleteAsync(int id);

    /// <summary>
    /// Searches for products matching a given search term.
    /// </summary>
    /// <param name="searchTerm">The term to search for (e.g., name or code).</param>
    /// <returns>A result containing a collection of matching product DTOs.</returns>
    Task<ResultOft<IEnumerable<ProductDto>>> SearchAsync(string searchTerm);

    /// <summary>
    /// Checks if a product exists by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <returns>True if the product exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(int id);
}