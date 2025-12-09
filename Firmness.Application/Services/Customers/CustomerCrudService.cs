namespace Firmness.Application.Services.Customers;

using System.Linq;
using AutoMapper;
using Firmness.Application.Common;
using Firmness.Application.DTOs.Customers;
using Firmness.Application.Interfaces;
using Firmness.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Service for handling Customer CRUD operations
/// </summary>
public class CustomerCrudService : ICustomerCrudService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomerCrudService> _logger;

    public CustomerCrudService(
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        ILogger<CustomerCrudService> logger)
    {
        _userManager = userManager;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ResultOft<IEnumerable<CustomerDto>>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        try
        {
            var customerUsers = await GetCustomerUsersAsync();

            var pagedUsers = customerUsers
                .OrderBy(u => u.UserName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var customerDtos = new List<CustomerDto>();

            foreach (var user in pagedUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var dto = _mapper.Map<CustomerDto>(user);
                dto.Roles = roles.ToList();
                customerDtos.Add(dto);
            }

            if (!customerDtos.Any())
                return ResultOft<IEnumerable<CustomerDto>>.Failure("No customers found on this page.");

            return ResultOft<IEnumerable<CustomerDto>>.Success(customerDtos);
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

            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return ResultOft<CustomerDto>.Failure($"Customer with ID {id} not found");

            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Contains("Customer"))
                return ResultOft<CustomerDto>.Failure($"Customer with ID {id} not found");

            var dto = _mapper.Map<CustomerDto>(user);
            dto.Roles = roles.ToList();

            return ResultOft<CustomerDto>.Success(dto);
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
            var customer = _mapper.Map<ApplicationUser>(createDto);
            
            var result = await _userManager.CreateAsync(customer, createDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return ResultOft<CustomerDto>.Failure(errors);
            }
            
            var roleResult = await _userManager.AddToRoleAsync(customer, "Customer");

            if (!roleResult.Succeeded)
            {
                var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                return ResultOft<CustomerDto>.Failure(errors);
            }

            var dto = _mapper.Map<CustomerDto>(customer);
            _logger.LogInformation("Customer '{CustomerName}' created with ID {CustomerId}", customer.UserName, customer.Id);

            return ResultOft<CustomerDto>.Success(dto);
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
            var customer = await _userManager.FindByIdAsync(updateDto.Id.ToString());
            if (customer == null)
            {
                _logger.LogWarning("Attempt to update non-existent customer with ID {CustomerId}", updateDto.Id);
                return ResultOft<CustomerDto>.Failure($"Customer with ID {updateDto.Id} not found");
            }

            _mapper.Map(updateDto, customer);

            if (!string.IsNullOrEmpty(updateDto.NewPassword))
            {
                var passwordResult = await _userManager.RemovePasswordAsync(customer);
                if (!passwordResult.Succeeded)
                {
                    return ResultOft<CustomerDto>.Failure("Error removing the old password.");
                }

                var newPasswordResult = await _userManager.AddPasswordAsync(customer, updateDto.NewPassword);
                if (!newPasswordResult.Succeeded)
                {
                    return ResultOft<CustomerDto>.Failure("Error setting the new password.");
                }
            }

            await _userManager.UpdateAsync(customer);

            var dto = _mapper.Map<CustomerDto>(customer);
            _logger.LogInformation("Updated customer '{CustomerName}' (ID: {CustomerId})", customer.UserName, customer.Id);

            return ResultOft<CustomerDto>.Success(dto);
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
            {
                return Result.Failure("The customer ID must be a valid GUID");
            }

            var customer = await _userManager.FindByIdAsync(id.ToString());
            if (customer == null)
            {
                _logger.LogWarning("Attempt to delete non-existent customer with ID {CustomerId}", id);
                return Result.Failure($"Customer with ID {id} not found");
            }

            var result = await _userManager.DeleteAsync(customer);

            if (!result.Succeeded)
            {
                return Result.Failure("Error deleting customer. Please try again.");
            }

            _logger.LogInformation("Customer with ID {CustomerId} removed", id);
            return Result.Success();
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
            {
                return ResultOft<IEnumerable<CustomerDto>>.Failure("The search term cannot be empty.");
            }

            if (searchTerm.Length < 2)
            {
                return ResultOft<IEnumerable<CustomerDto>>.Failure("The search term must be at least 2 characters long");
            }

            // ✅ OPTIMIZED: Filter in database, not in memory
            var filtered = _userManager.Users
                .Where(p => 
                    p.UserName.Contains(searchTerm) || 
                    p.FullName.Contains(searchTerm))
                .Take(50); // Safety limit

            var customers = await filtered.ToListAsync();

            var dtos = _mapper.Map<IEnumerable<CustomerDto>>(customers);
            _logger.LogInformation("Searching for customers with the term '{SearchTerm}' returned {Count} results", searchTerm, dtos.Count());
            return ResultOft<IEnumerable<CustomerDto>>.Success(dtos);
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
            var customer = await _userManager.FindByIdAsync(id.ToString());
            return customer != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if customer exists {CustomerId}", id);
            return false;
        }
    }

    private async Task<List<ApplicationUser>> GetCustomerUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var customers = new List<ApplicationUser>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Customer"))
                customers.Add(user);
        }

        return customers;
    }
}
