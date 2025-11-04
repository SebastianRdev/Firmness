namespace Firmness.Application.Services;

using AutoMapper;
using Firmness.Application.Common;
using Firmness.Application.DTOs.Products;
using Firmness.Application.Interfaces;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Microsoft.Extensions.Logging;

/// <summary>
/// Provides business logic operations for managing products,
/// including creation, retrieval, updating, deletion, and searching.
/// </summary>
public class ProductService : IProductService
{
    private readonly IGenericRepository<Product> _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductService"/> class.
    /// </summary>
    /// <param name="productRepository">The repository used for data access of products.</param>
    /// <param name="mapper">The AutoMapper instance used for object mapping.</param>
    /// <param name="logger">The logger used to record application events and errors.</param>
    public ProductService(
        IGenericRepository<Product> productRepository,
        IMapper mapper,
        ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all products.
    /// </summary>
    /// <returns>
    /// A <see cref="ResultOft{T}"/> containing a collection of <see cref="ProductDto"/> 
    /// if successful, or an error message if failed.
    /// </returns>
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
            _logger.LogError(ex, "Error retrieving all products");
            return ResultOft<IEnumerable<ProductDto>>.Failure("Error loading products. Please try again.");
        }
    }

    /// <summary>
    /// Retrieves a product by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <returns>
    /// A <see cref="ResultOft{T}"/> containing the <see cref="ProductDto"/> 
    /// if found, or an error message otherwise.
    /// </returns>
    public async Task<ResultOft<ProductDto>> GetByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return ResultOft<ProductDto>.Failure("The product ID must be greater than 0");
            }

            var product = await _productRepository.GetByIdAsync(id);
            
            if (product == null)
            {
                _logger.LogWarning("Product with ID {{ProductId}} not found", id);
                return ResultOft<ProductDto>.Failure($"Product with ID {id} not found");
            }

            var dto = _mapper.Map<ProductDto>(product);
            return ResultOft<ProductDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obtaining the product {ProductId}", id);
            return ResultOft<ProductDto>.Failure("Error loading product. Please try again.");
        }
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="createDto">The product data transfer object containing creation details.</param>
    /// <returns>
    /// A <see cref="ResultOft{T}"/> containing the created <see cref="ProductDto"/> 
    /// if successful, or an error message if failed.
    /// </returns>
    public async Task<ResultOft<ProductDto>> CreateAsync(CreateProductDto createDto)
    {
        try
        {
            var allProducts = await _productRepository.GetAllAsync();
            if (allProducts.Any(p => p.Code.Equals(createDto.Code, StringComparison.OrdinalIgnoreCase)))
            {
                return ResultOft<ProductDto>.Failure($"A product with the code already exists. '{createDto.Code}'");
            }

            // Map and assign automatic values
            var product = _mapper.Map<Product>(createDto);
            product.CreatedAt = DateTime.UtcNow;
            product.IsActive = true;

            // Save
            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            // Return succesfully result
            var dto = _mapper.Map<ProductDto>(product);
            _logger.LogInformation("Product '{ProductName}' created with ID {ProductId}", product.Name, product.Id);
            return ResultOft<ProductDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return ResultOft<ProductDto>.Failure("Error creating product. Please try again.");
        }
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="updateDto">The product data transfer object containing update details.</param>
    /// <returns>
    /// A <see cref="ResultOft{T}"/> containing the updated <see cref="ProductDto"/> 
    /// if successful, or an error message if failed.
    /// </returns>
    public async Task<ResultOft<ProductDto>> UpdateAsync(UpdateProductDto updateDto)
    {
        try
        {
            if (updateDto.Id <= 0)
            {
                return ResultOft<ProductDto>.Failure("The product ID must be greater than 0");
            }
            
            var product = await _productRepository.GetByIdAsync(updateDto.Id);
            if (product == null)
            {
                _logger.LogWarning("Attempt to update non-existent product with ID {ProductId}", updateDto.Id);
                return ResultOft<ProductDto>.Failure($"Product with ID {updateDto.Id} not found");
            }

            // Validate unique code (excluding the current product)
            var allProducts = await _productRepository.GetAllAsync();
            if (allProducts.Any(p => 
                p.Id != updateDto.Id && 
                p.Code.Equals(updateDto.Code, StringComparison.OrdinalIgnoreCase)))
            {
                return ResultOft<ProductDto>.Failure($"A product with the code already exists. '{updateDto.Code}'");
            }

            // Map changes
            _mapper.Map(updateDto, product);

            // Save
            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();

            // Return result
            var dto = _mapper.Map<ProductDto>(product);
            _logger.LogInformation("Updated '{ProductName}' product (ID: {ProductId})", product.Name, product.Id);
            return ResultOft<ProductDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Product update failed {ProductId}", updateDto.Id);
            return ResultOft<ProductDto>.Failure("Product update failed. Please try again.");
        }
    }

    /// <summary>
    /// Deletes a product by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product to delete.</param>
    /// <returns>
    /// A <see cref="Result"/> indicating success or failure.
    /// </returns>
    public async Task<Result> DeleteAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return Result.Failure("The product ID must be greater than 0");
            }
            
            var exists = await _productRepository.ExistsAsync(id);
            if (!exists)
            {
                _logger.LogWarning("Attempt to delete non-existent product with ID {ProductId}", id);
                return Result.Failure($"Product with ID {id} not found");
            }

            // Delete
            await _productRepository.DeleteAsync(id);
            await _productRepository.SaveChangesAsync();

            _logger.LogInformation("Product with ID {ProductId} removed", id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting the product {ProductId}", id);
            return Result.Failure("Error deleting the product. Please try again..");
        }
    }

    /// <summary>
    /// Searches for products that match the given term in their name or code.
    /// </summary>
    /// <param name="searchTerm">The search term used to filter products.</param>
    /// <returns>
    /// A <see cref="ResultOft{T}"/> containing a filtered list of <see cref="ProductDto"/> 
    /// if successful, or an error message if failed.
    /// </returns>
    public async Task<ResultOft<IEnumerable<ProductDto>>> SearchAsync(string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return ResultOft<IEnumerable<ProductDto>>.Failure("The search term cannot be empty.");
            }

            if (searchTerm.Length < 2)
            {
                return ResultOft<IEnumerable<ProductDto>>.Failure("The search term must be at least 2 characters long");
            }

            var products = await _productRepository.GetAllAsync();
            
            var filtered = products.Where(p => 
                p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.Code.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            );

            var dtos = _mapper.Map<IEnumerable<ProductDto>>(filtered);
            _logger.LogInformation("Searching for products with the term '{SearchTerm}' returned {Count} results", searchTerm, dtos.Count());
            return ResultOft<IEnumerable<ProductDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for products with term '{SearchTerm}'", searchTerm);
            return ResultOft<IEnumerable<ProductDto>>.Failure("Error searching for products. Please try again.");
        }
    }

    /// <summary>
    /// Checks whether a product with the specified identifier exists.
    /// </summary>
    /// <param name="id">The product identifier to check.</param>
    /// <returns>
    /// A boolean indicating whether the product exists.
    /// </returns>
    public async Task<bool> ExistsAsync(int id)
    {
        try
        {
            return await _productRepository.ExistsAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "A product with the code already exists. {ProductId}", id);
            return false;
        }
    }
}