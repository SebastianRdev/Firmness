namespace Firmness.WebAdmin.ApiClients;

using Firmness.Application.DTOs.Products;
using Firmness.Application.Common;

/// <summary>
/// Cliente HTTP para consumir la API de productos.
/// Abstracción que permite cambiar la implementación sin afectar los controllers.
/// </summary>
public interface IProductApiClient
{
    /// <summary>
    /// Obtiene todos los productos desde la API.
    /// </summary>
    Task<ResultOft<IEnumerable<ProductDto>>> GetAllAsync();

    /// <summary>
    /// Obtiene un producto por su ID desde la API.
    /// </summary>
    Task<ResultOft<ProductDto>> GetByIdAsync(int id);

    /// <summary>
    /// Crea un nuevo producto en la API.
    /// </summary>
    Task<ResultOft<ProductDto>> CreateAsync(CreateProductDto createDto);

    /// <summary>
    /// Actualiza un producto existente en la API.
    /// </summary>
    Task<ResultOft<ProductDto>> UpdateAsync(UpdateProductDto updateDto);

    /// <summary>
    /// Elimina un producto desde la API.
    /// </summary>
    Task<Result> DeleteAsync(int id);

    /// <summary>
    /// Busca productos por término desde la API.
    /// </summary>
    Task<ResultOft<IEnumerable<ProductDto>>> SearchAsync(string searchTerm);
}