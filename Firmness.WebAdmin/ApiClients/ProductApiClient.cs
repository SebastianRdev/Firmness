namespace Firmness.WebAdmin.ApiClients;

using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Firmness.Application.DTOs.Products;
using Firmness.Application.Common;

/// <summary>
/// Implementación del cliente HTTP para productos.
/// Maneja la comunicación con la API REST.
/// </summary>
public class ProductApiClient : IProductApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductApiClient> _logger;

    public ProductApiClient(HttpClient httpClient, ILogger<ProductApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ResultOft<IEnumerable<ProductDto>>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("products");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("API error getting products: {StatusCode} - {Error}", 
                    response.StatusCode, errorContent);
                return ResultOft<IEnumerable<ProductDto>>.Failure("Error loading products from API");
            }

            var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();
            return ResultOft<IEnumerable<ProductDto>>.Success(products ?? new List<ProductDto>());
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error calling API");
            return ResultOft<IEnumerable<ProductDto>>.Failure("Could not connect to API. Please verify the service is running.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting products");
            return ResultOft<IEnumerable<ProductDto>>.Failure("Unexpected error. Please try again.");
        }
    }

    public async Task<ResultOft<ProductDto>> GetByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"products/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return ResultOft<ProductDto>.Failure($"Product with ID {id} not found");
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("API error getting product {Id}: {StatusCode} - {Error}", 
                    id, response.StatusCode, errorContent);
                return ResultOft<ProductDto>.Failure("Error loading product from API");
            }

            var product = await response.Content.ReadFromJsonAsync<ProductDto>();
            return product != null 
                ? ResultOft<ProductDto>.Success(product)
                : ResultOft<ProductDto>.Failure("Product not found");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error calling API");
            return ResultOft<ProductDto>.Failure("Could not connect to API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting product {Id}", id);
            return ResultOft<ProductDto>.Failure("Unexpected error. Please try again.");
        }
    }

    public async Task<ResultOft<ProductDto>> CreateAsync(CreateProductDto createDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("products", createDto);

            if (!response.IsSuccessStatusCode)
            {
                // La API devuelve { "error": "mensaje" }
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                _logger.LogWarning("API rejected product creation: {Error}", errorResponse?.Error);
                return ResultOft<ProductDto>.Failure(errorResponse?.Error ?? "Error creating product");
            }

            var product = await response.Content.ReadFromJsonAsync<ProductDto>();
            return product != null
                ? ResultOft<ProductDto>.Success(product)
                : ResultOft<ProductDto>.Failure("Product created but could not retrieve data");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error calling API");
            return ResultOft<ProductDto>.Failure("Could not connect to API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating product");
            return ResultOft<ProductDto>.Failure("Unexpected error. Please try again.");
        }
    }

    public async Task<ResultOft<ProductDto>> UpdateAsync(UpdateProductDto updateDto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"products/{updateDto.Id}", updateDto);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return ResultOft<ProductDto>.Failure($"Product with ID {updateDto.Id} not found");
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                _logger.LogWarning("API rejected product update: {Error}", errorResponse?.Error);
                return ResultOft<ProductDto>.Failure(errorResponse?.Error ?? "Error updating product");
            }

            var product = await response.Content.ReadFromJsonAsync<ProductDto>();
            return product != null
                ? ResultOft<ProductDto>.Success(product)
                : ResultOft<ProductDto>.Failure("Product updated but could not retrieve data");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error calling API");
            return ResultOft<ProductDto>.Failure("Could not connect to API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating product");
            return ResultOft<ProductDto>.Failure("Unexpected error. Please try again.");
        }
    }

    // ========================================
    // DELETE
    // ========================================
    public async Task<Result> DeleteAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"products/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return Result.Failure($"Product with ID {id} not found");
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                _logger.LogWarning("API rejected product deletion: {Error}", errorResponse?.Error);
                return Result.Failure(errorResponse?.Error ?? "Error deleting product");
            }

            return Result.Success();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error calling API");
            return Result.Failure("Could not connect to API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting product");
            return Result.Failure("Unexpected error. Please try again.");
        }
    }

    public async Task<ResultOft<IEnumerable<ProductDto>>> SearchAsync(string searchTerm)
    {
        try
        {
            var response = await _httpClient.GetAsync($"products/search?term={Uri.EscapeDataString(searchTerm)}");

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                _logger.LogWarning("API search error: {Error}", errorResponse?.Error);
                return ResultOft<IEnumerable<ProductDto>>.Failure(errorResponse?.Error ?? "Error searching products");
            }

            var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();
            return ResultOft<IEnumerable<ProductDto>>.Success(products ?? new List<ProductDto>());
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error calling API");
            return ResultOft<IEnumerable<ProductDto>>.Failure("Could not connect to API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error searching products");
            return ResultOft<IEnumerable<ProductDto>>.Failure("Unexpected error. Please try again.");
        }
    }

    // ========================================
    // HELPER CLASS (errores de la API)
    // ========================================
    private class ApiErrorResponse
    {
        public string? Error { get; set; }
    }
}