// Firmness.Application/Interfaces/IProductService.cs
using Firmness.Application.DTOs.Products;
using Firmness.Application.Common;

namespace Firmness.Application.Interfaces;

public interface IProductService
{
    /// <summary>
    /// Obtiene todos los productos del sistema.
    /// </summary>
    Task<ResultOft<IEnumerable<ProductDto>>> GetAllAsync();

    /// <summary>
    /// Obtiene un producto por su ID.
    /// </summary>
    Task<ResultOft<ProductDto>> GetByIdAsync(int id);

    /// <summary>
    /// Crea un nuevo producto.
    /// </summary>
    Task<ResultOft<ProductDto>> CreateAsync(CreateProductDto createDto);

    /// <summary>
    /// Actualiza un producto existente.
    /// </summary>
    Task<ResultOft<ProductDto>> UpdateAsync(UpdateProductDto updateDto);

    /// <summary>
    /// Elimina un producto.
    /// </summary>
    Task<Result> DeleteAsync(int id);

    /// <summary>
    /// Busca productos por término.
    /// </summary>
    Task<ResultOft<IEnumerable<ProductDto>>> SearchAsync(string searchTerm);

    /// <summary>
    /// Verifica si existe un producto.
    /// </summary>
    Task<bool> ExistsAsync(int id);
}