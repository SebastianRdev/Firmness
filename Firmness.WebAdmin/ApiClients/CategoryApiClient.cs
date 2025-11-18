namespace Firmness.WebAdmin.ApiClients;

using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Firmness.Application.DTOs.Categories;
using Firmness.Application.Common;

public class CategoryApiClient : ICategoryApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CategoryApiClient> _logger;

    public CategoryApiClient(HttpClient httpClient, ILogger<CategoryApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<ResultOft<IEnumerable<CategoryDto>>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("categories");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("API error getting categories: {StatusCode} - {Error}", 
                    response.StatusCode, errorContent);
                return ResultOft<IEnumerable<CategoryDto>>.Failure("Error loading categories from API");
            }

            var categories = await response.Content.ReadFromJsonAsync<IEnumerable<CategoryDto>>();
            return ResultOft<IEnumerable<CategoryDto>>.Success(categories ?? new List<CategoryDto>());
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error calling API");
            return ResultOft<IEnumerable<CategoryDto>>.Failure("Could not connect to API. Please verify the service is running.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting categories");
            return ResultOft<IEnumerable<CategoryDto>>.Failure("Unexpected error. Please try again.");
        }
    }

    public async Task<ResultOft<CategoryDto>> GetByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"categories/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return ResultOft<CategoryDto>.Failure($"Category with ID {id} not found");
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("API error getting category {Id}: {StatusCode} - {Error}", 
                    id, response.StatusCode, errorContent);
                return ResultOft<CategoryDto>.Failure("Error loading category from API");
            }

            var category = await response.Content.ReadFromJsonAsync<CategoryDto>();
            return category != null 
                ? ResultOft<CategoryDto>.Success(category)
                : ResultOft<CategoryDto>.Failure("Category not found");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error calling API");
            return ResultOft<CategoryDto>.Failure("Could not connect to API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting category {Id}", id);
            return ResultOft<CategoryDto>.Failure("Unexpected error. Please try again.");
        }
    }

    public async Task<ResultOft<CategoryDto>> CreateAsync(CreateCategoryDto createDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("categories", createDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                _logger.LogWarning("API rejected category creation: {Error}", errorResponse?.Error);
                return ResultOft<CategoryDto>.Failure(errorResponse?.Error ?? "Error creating category");
            }

            var category = await response.Content.ReadFromJsonAsync<CategoryDto>();
            return category != null
                ? ResultOft<CategoryDto>.Success(category)
                : ResultOft<CategoryDto>.Failure("Category created but could not retrieve data");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error calling API");
            return ResultOft<CategoryDto>.Failure("Could not connect to API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating category");
            return ResultOft<CategoryDto>.Failure("Unexpected error. Please try again.");
        }
    }

    public async Task<ResultOft<CategoryDto>> UpdateAsync(UpdateCategoryDto updateDto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"categories/{updateDto.Id}", updateDto);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return ResultOft<CategoryDto>.Failure($"Category with ID {updateDto.Id} not found");
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                _logger.LogWarning("API rejected category update: {Error}", errorResponse?.Error);
                return ResultOft<CategoryDto>.Failure(errorResponse?.Error ?? "Error updating category");
            }

            var category = await response.Content.ReadFromJsonAsync<CategoryDto>();
            return category != null
                ? ResultOft<CategoryDto>.Success(category)
                : ResultOft<CategoryDto>.Failure("Category updated but could not retrieve data");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error calling API");
            return ResultOft<CategoryDto>.Failure("Could not connect to API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating category");
            return ResultOft<CategoryDto>.Failure("Unexpected error. Please try again.");
        }
    }

    // ========================================
    // DELETE
    // ========================================
    public async Task<Result> DeleteAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"categories/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return Result.Failure($"Category with ID {id} not found");
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                _logger.LogWarning("API rejected category deletion: {Error}", errorResponse?.Error);
                return Result.Failure(errorResponse?.Error ?? "Error deleting category");
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
            _logger.LogError(ex, "Unexpected error deleting category");
            return Result.Failure("Unexpected error. Please try again.");
        }
    }

    public async Task<ResultOft<IEnumerable<CategoryDto>>> SearchAsync(string searchTerm)
    {
        try
        {
            var response = await _httpClient.GetAsync($"categories/search?term={Uri.EscapeDataString(searchTerm)}");

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                _logger.LogWarning("API search error: {Error}", errorResponse?.Error);
                return ResultOft<IEnumerable<CategoryDto>>.Failure(errorResponse?.Error ?? "Error searching categories");
            }

            var categories = await response.Content.ReadFromJsonAsync<IEnumerable<CategoryDto>>();
            return ResultOft<IEnumerable<CategoryDto>>.Success(categories ?? new List<CategoryDto>());
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error calling API");
            return ResultOft<IEnumerable<CategoryDto>>.Failure("Could not connect to API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error searching categories");
            return ResultOft<IEnumerable<CategoryDto>>.Failure("Unexpected error. Please try again.");
        }
    }

    // ========================================
    // HELPER CLASS (API errors)
    // ========================================
    private class ApiErrorResponse
    {
        public string? Error { get; set; }
    }
}