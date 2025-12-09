namespace Firmness.Application.Services.Customers;

using System.Linq;
using Firmness.Application.Common;
using Firmness.Application.DTOs.Customers;
using Firmness.Application.Interfaces;
using Microsoft.Extensions.Logging;

/// <summary>
/// Service for handling Customer CRUD operations using IIdentityService abstraction
/// </summary>
public class CustomerCrudService : ICustomerCrudService
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<CustomerCrudService> _logger;

    public CustomerCrudService(
        IIdentityService identityService,
        ILogger<CustomerCrudService> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<ResultOft<IEnumerable<CustomerDto>>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        try
        {
            // ✅ REFACTORED: Now uses IIdentityService with optimized N+1 query resolution
            return await _identityService.GetCustomerUsersAsync(page, pageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paginated customers.");
            return ResultOft<IEnumerable<CustomerDto>>.Failure("Error loading paginated customers.");
        }
    }

    public async Task<ResultOft<CustomerDto>> GetByIdAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return ResultOft<CustomerDto>.Failure("The customer ID must be a valid GUID");

            var result = await _identityService.GetUserByIdAsync(id);

            if (!result.IsSuccess)
                return result;

            // Verify user has Customer role
            if (!result.Data.Roles.Contains("Customer"))
                return ResultOft<CustomerDto>.Failure($"Customer with ID {id} not found");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obtaining the customer {CustomerId}", id);
            return ResultOft<CustomerDto>.Failure("Error loading customer. Please try again.");
        }
    }

    public async Task<ResultOft<CustomerDto>> CreateAsync(CreateCustomerDto createDto)
    {
        try
        {
            return await _identityService.CreateUserAsync(createDto, createDto.Password);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            return ResultOft<CustomerDto>.Failure("Error creating customer. Please try again.");
        }
    }

    public async Task<ResultOft<CustomerDto>> UpdateAsync(UpdateCustomerDto updateDto)
    {
        try
        {
            // Update user information
            var updateResult = await _identityService.UpdateUserAsync(updateDto.Id, updateDto);

            if (!updateResult.IsSuccess)
                return updateResult;

            // Change password if provided
            if (!string.IsNullOrEmpty(updateDto.NewPassword))
            {
                var passwordResult = await _identityService.ChangePasswordAsync(updateDto.Id, updateDto.NewPassword);
                if (!passwordResult.IsSuccess)
                    return ResultOft<CustomerDto>.Failure(passwordResult.ErrorMessage);
            }

            _logger.LogInformation("Updated customer (ID: {CustomerId})", updateDto.Id);
            return updateResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Customer update failed {CustomerId}", updateDto.Id);
            return ResultOft<CustomerDto>.Failure("Customer update failed. Please try again.");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return Result.Failure("The customer ID must be a valid GUID");

            return await _identityService.DeleteUserAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting the customer {CustomerId}", id);
            return Result.Failure("Error deleting the customer. Please try again.");
        }
    }

    public async Task<ResultOft<IEnumerable<CustomerDto>>> SearchAsync(string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return ResultOft<IEnumerable<CustomerDto>>.Failure("The search term cannot be empty.");

            if (searchTerm.Length < 2)
                return ResultOft<IEnumerable<CustomerDto>>.Failure("The search term must be at least 2 characters long");

            return await _identityService.SearchUsersAsync(searchTerm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for customers with term '{SearchTerm}'", searchTerm);
            return ResultOft<IEnumerable<CustomerDto>>.Failure("Error searching for customers. Please try again.");
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        try
        {
            return await _identityService.UserExistsAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if customer exists {CustomerId}", id);
            return false;
        }
    }
}
