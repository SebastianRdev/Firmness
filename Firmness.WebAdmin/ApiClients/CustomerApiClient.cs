namespace Firmness.WebAdmin.ApiClients;

using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Firmness.Application.DTOs.Customers;
using Firmness.Application.Common;
using Firmness.Application.DTOs.Excel;

public class CustomerApiClient : ICustomerApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CustomerApiClient> _logger;

    public CustomerApiClient(HttpClient httpClient, ILogger<CustomerApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<ResultOft<IEnumerable<CustomerDto>>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("customers");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("API error getting customers: {StatusCode} - {Error}", 
                    response.StatusCode, errorContent);
                return ResultOft<IEnumerable<CustomerDto>>.Failure("Error loading customers from API");
            }

            var customers = await response.Content.ReadFromJsonAsync<IEnumerable<CustomerDto>>();
            return ResultOft<IEnumerable<CustomerDto>>.Success(customers ?? new List<CustomerDto>());
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error calling API");
            return ResultOft<IEnumerable<CustomerDto>>.Failure("Could not connect to API. Please verify the service is running.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting customers");
            return ResultOft<IEnumerable<CustomerDto>>.Failure("Unexpected error. Please try again.");
        }
    }

    public async Task<ResultOft<CustomerDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"customers/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return ResultOft<CustomerDto>.Failure($"Customer with ID {id} not found");
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("API error getting customer {Id}: {StatusCode} - {Error}", 
                    id, response.StatusCode, errorContent);
                return ResultOft<CustomerDto>.Failure("Error loading customer from API");
            }

            var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
            return customer != null 
                ? ResultOft<CustomerDto>.Success(customer)
                : ResultOft<CustomerDto>.Failure("Customer not found");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error calling API");
            return ResultOft<CustomerDto>.Failure("Could not connect to API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting customer {Id}", id);
            return ResultOft<CustomerDto>.Failure("Unexpected error. Please try again.");
        }
    }

    public async Task<ResultOft<CustomerDto>> CreateAsync(CreateCustomerDto createDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("customers", createDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                _logger.LogWarning("API rejected customer creation: {Error}", errorResponse?.Error);
                return ResultOft<CustomerDto>.Failure(errorResponse?.Error ?? "Error creating customer");
            }

            var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
            return customer != null
                ? ResultOft<CustomerDto>.Success(customer)
                : ResultOft<CustomerDto>.Failure("Customer created but could not retrieve data");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error calling API");
            return ResultOft<CustomerDto>.Failure("Could not connect to API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating customer");
            return ResultOft<CustomerDto>.Failure("Unexpected error. Please try again.");
        }
    }

    public async Task<ResultOft<CustomerDto>> UpdateAsync(UpdateCustomerDto updateDto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"customers/{updateDto.Id}", updateDto);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return ResultOft<CustomerDto>.Failure($"Customer with ID {updateDto.Id} not found");
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                _logger.LogWarning("API rejected customer update: {Error}", errorResponse?.Error);
                return ResultOft<CustomerDto>.Failure(errorResponse?.Error ?? "Error updating customer");
            }

            var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
            return customer != null
                ? ResultOft<CustomerDto>.Success(customer)
                : ResultOft<CustomerDto>.Failure("Customer updated but could not retrieve data");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error calling API");
            return ResultOft<CustomerDto>.Failure("Could not connect to API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating customer");
            return ResultOft<CustomerDto>.Failure("Unexpected error. Please try again.");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"customers/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return Result.Failure($"Customer with ID {id} not found");
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                _logger.LogWarning("API rejected customer deletion: {Error}", errorResponse?.Error);
                return Result.Failure(errorResponse?.Error ?? "Error deleting customer");
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
            _logger.LogError(ex, "Unexpected error deleting customer");
            return Result.Failure("Unexpected error. Please try again.");
        }
    }

    public async Task<ResultOft<IEnumerable<CustomerDto>>> SearchAsync(string searchTerm)
    {
        try
        {
            var response = await _httpClient.GetAsync($"customers/search?term={Uri.EscapeDataString(searchTerm)}");

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                _logger.LogWarning("API search error: {Error}", errorResponse?.Error);
                return ResultOft<IEnumerable<CustomerDto>>.Failure(errorResponse?.Error ?? "Error searching customers");
            }

            var customers = await response.Content.ReadFromJsonAsync<IEnumerable<CustomerDto>>();
            return ResultOft<IEnumerable<CustomerDto>>.Success(customers ?? new List<CustomerDto>());
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error calling API");
            return ResultOft<IEnumerable<CustomerDto>>.Failure("Could not connect to API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error searching customers");
            return ResultOft<IEnumerable<CustomerDto>>.Failure("Unexpected error. Please try again.");
        }
    }
    
    public async Task<ResultOft<IEnumerable<string>>> GetAllRolesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("customers/roles");
        
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("API error getting roles: {StatusCode} - {Error}", 
                    response.StatusCode, errorContent);
                return ResultOft<IEnumerable<string>>.Failure("Error loading roles from API");
            }

            // API returns IEnumerable<string> directly, not wrapped in ResultOft
            var roles = await response.Content.ReadFromJsonAsync<IEnumerable<string>>();
            return ResultOft<IEnumerable<string>>.Success(roles ?? new List<string>());
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error calling API");
            return ResultOft<IEnumerable<string>>.Failure("Could not connect to API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting roles");
            return ResultOft<IEnumerable<string>>.Failure("Unexpected error. Please try again.");
        }
    }
    
    public async Task<Result> UpdateUserRoleAsync(Guid id, string selectedRole)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"customers/{id}/roles", selectedRole);
        
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("API error updating user role: {StatusCode} - {Error}", 
                    response.StatusCode, errorContent);
                return Result.Failure("Error updating user role");
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
            _logger.LogError(ex, "Unexpected error updating user role");
            return Result.Failure("Unexpected error. Please try again.");
        }
    }
    
    public async Task<ResultOft<IEnumerable<CustomerDto>>> GetAllPaginatedAsync(int page = 1, int pageSize = 10)
    {
        try
        {
            var response = await _httpClient.GetAsync($"customers?page={page}&pageSize={pageSize}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("API error getting paginated customers: {StatusCode} - {Error}", 
                    response.StatusCode, errorContent);
                return ResultOft<IEnumerable<CustomerDto>>.Failure("Error loading paginated customers from API");
            }

            var customers = await response.Content.ReadFromJsonAsync<IEnumerable<CustomerDto>>();
            return ResultOft<IEnumerable<CustomerDto>>.Success(customers ?? new List<CustomerDto>());
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error calling API");
            return ResultOft<IEnumerable<CustomerDto>>.Failure("Could not connect to API. Please verify the service is running.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting paginated customers");
            return ResultOft<IEnumerable<CustomerDto>>.Failure("Unexpected error. Please try again.");
        }
    }
    
    public async Task<Result> ImportExcelAsync(IFormFile file)
    {
        try
        {
            var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(file.OpenReadStream());
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            content.Add(fileContent, "file", file.FileName);

            var response = await _httpClient.PostAsync("customers/import-excel", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("API error importing Excel file: {StatusCode} - {Error}",
                    response.StatusCode, errorContent);
                return Result.Failure("Error importing Excel file.");
            }

            return Result.Success();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error calling API");
            return Result.Failure("Could not connect to API.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error importing Excel");
            return Result.Failure("Unexpected error. Please try again.");
        }
    }

    public async Task<ResultOft<ExcelHeadersResponseDto>> ExtractHeadersFromExcelAsync(IFormFile file)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            using var stream = file.OpenReadStream();
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
            content.Add(streamContent, "file", file.FileName);

            var response = await _httpClient.PostAsync("customers/import/headers", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("API rejected header extraction: {StatusCode} - {Error}", response.StatusCode, errorContent);
                return ResultOft<ExcelHeadersResponseDto>.Failure("Error extracting headers from API");
            }

            // Leemos el ResultOft completo, no solo el DTO
            var resultWrapper = await response.Content.ReadFromJsonAsync<ResultOft<ExcelHeadersResponseDto>>();
        
            // Devolvemos el resultado tal cual viene de la API
            return resultWrapper ?? ResultOft<ExcelHeadersResponseDto>.Failure("No data returned from API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Network error extracting headers from API");
            return ResultOft<ExcelHeadersResponseDto>.Failure("Could not connect to API");
        }
    }
    

    // HELPER CLASS (API errors)
    private class ApiErrorResponse
    {
        public string? Error { get; set; }
    }
}