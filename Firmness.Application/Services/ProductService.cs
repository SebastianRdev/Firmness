// Firmness.Application/Services/ProductService.cs
using AutoMapper;
using Firmness.Application.Common;
using Firmness.Application.DTOs.Products;
using Firmness.Application.Interfaces;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Firmness.Application.Services;

public class ProductService : IProductService
{
    private readonly IGenericRepository<Product> _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IGenericRepository<Product> productRepository,
        IMapper mapper,
        ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ResultOft<IEnumerable<ProductDto>>> GetAllAsync()
    {
        try
        {
            var products = await _productRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return ResultOft<IEnumerable<ProductDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los productos");
            return ResultOft<IEnumerable<ProductDto>>.Failure("Error al cargar los productos. Por favor, intente nuevamente.");
        }
    }

    public async Task<ResultOft<ProductDto>> GetByIdAsync(int id)
    {
        try
        {
            // Validación de negocio
            if (id <= 0)
            {
                return ResultOft<ProductDto>.Failure("El ID del producto debe ser mayor a 0");
            }

            var product = await _productRepository.GetByIdAsync(id);
            
            if (product == null)
            {
                _logger.LogWarning("Producto con ID {ProductId} no encontrado", id);
                return ResultOft<ProductDto>.Failure($"Producto con ID {id} no encontrado");
            }

            var dto = _mapper.Map<ProductDto>(product);
            return ResultOft<ProductDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el producto {ProductId}", id);
            return ResultOft<ProductDto>.Failure("Error al cargar el producto. Por favor, intente nuevamente.");
        }
    }

    public async Task<ResultOft<ProductDto>> CreateAsync(CreateProductDto createDto)
    {
        try
        {
            // Validaciones de negocio adicionales (más allá de Data Annotations)
            
            // Ejemplo: Verificar que el código no exista
            var allProducts = await _productRepository.GetAllAsync();
            if (allProducts.Any(p => p.Code.Equals(createDto.Code, StringComparison.OrdinalIgnoreCase)))
            {
                return ResultOft<ProductDto>.Failure($"Ya existe un producto con el código '{createDto.Code}'");
            }

            // Mapear y asignar valores automáticos
            var product = _mapper.Map<Product>(createDto);
            product.CreatedAt = DateTime.UtcNow;
            product.IsActive = true;

            // Guardar
            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            // Devolver resultado exitoso
            var dto = _mapper.Map<ProductDto>(product);
            _logger.LogInformation("Producto '{ProductName}' creado con ID {ProductId}", product.Name, product.Id);
            return ResultOft<ProductDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el producto");
            return ResultOft<ProductDto>.Failure("Error al crear el producto. Por favor, intente nuevamente.");
        }
    }

    public async Task<ResultOft<ProductDto>> UpdateAsync(UpdateProductDto updateDto)
    {
        try
        {
            // Validación de ID
            if (updateDto.Id <= 0)
            {
                return ResultOft<ProductDto>.Failure("El ID del producto debe ser mayor a 0");
            }

            // Verificar que exista
            var product = await _productRepository.GetByIdAsync(updateDto.Id);
            if (product == null)
            {
                _logger.LogWarning("Intento de actualizar producto inexistente con ID {ProductId}", updateDto.Id);
                return ResultOft<ProductDto>.Failure($"Producto con ID {updateDto.Id} no encontrado");
            }

            // Validar código único (excluyendo el producto actual)
            var allProducts = await _productRepository.GetAllAsync();
            if (allProducts.Any(p => 
                p.Id != updateDto.Id && 
                p.Code.Equals(updateDto.Code, StringComparison.OrdinalIgnoreCase)))
            {
                return ResultOft<ProductDto>.Failure($"Ya existe otro producto con el código '{updateDto.Code}'");
            }

            // Mapear cambios
            _mapper.Map(updateDto, product);

            // Guardar
            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();

            // Devolver resultado
            var dto = _mapper.Map<ProductDto>(product);
            _logger.LogInformation("Producto '{ProductName}' actualizado (ID: {ProductId})", product.Name, product.Id);
            return ResultOft<ProductDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el producto {ProductId}", updateDto.Id);
            return ResultOft<ProductDto>.Failure("Error al actualizar el producto. Por favor, intente nuevamente.");
        }
    }

    public async Task<Result> DeleteAsync(int id)
    {
        try
        {
            // Validación de ID
            if (id <= 0)
            {
                return Result.Failure("El ID del producto debe ser mayor a 0");
            }

            // Verificar que exista
            var exists = await _productRepository.ExistsAsync(id);
            if (!exists)
            {
                _logger.LogWarning("Intento de eliminar producto inexistente con ID {ProductId}", id);
                return Result.Failure($"Producto con ID {id} no encontrado");
            }

            // Validación de negocio: ¿Se puede eliminar?
            // Ejemplo: verificar que no tenga ventas asociadas
            // (esto lo harías cuando tengas la entidad Sale implementada)

            // Eliminar
            await _productRepository.DeleteAsync(id);
            await _productRepository.SaveChangesAsync();

            _logger.LogInformation("Producto con ID {ProductId} eliminado", id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el producto {ProductId}", id);
            return Result.Failure("Error al eliminar el producto. Por favor, intente nuevamente.");
        }
    }

    public async Task<ResultOft<IEnumerable<ProductDto>>> SearchAsync(string searchTerm)
    {
        try
        {
            // Validación
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return ResultOft<IEnumerable<ProductDto>>.Failure("El término de búsqueda no puede estar vacío");
            }

            if (searchTerm.Length < 2)
            {
                return ResultOft<IEnumerable<ProductDto>>.Failure("El término de búsqueda debe tener al menos 2 caracteres");
            }

            var products = await _productRepository.GetAllAsync();
            
            var filtered = products.Where(p => 
                p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.Code.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            );

            var dtos = _mapper.Map<IEnumerable<ProductDto>>(filtered);
            _logger.LogInformation("Búsqueda de productos con término '{SearchTerm}' devolvió {Count} resultados", searchTerm, dtos.Count());
            return ResultOft<IEnumerable<ProductDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar productos con término '{SearchTerm}'", searchTerm);
            return ResultOft<IEnumerable<ProductDto>>.Failure("Error al buscar productos. Por favor, intente nuevamente.");
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        try
        {
            return await _productRepository.ExistsAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar existencia del producto {ProductId}", id);
            return false; // En caso de error, asumimos que no existe
        }
    }
}