using AutoMapper;
using Firmness.Application.Common;
using Firmness.Application.DTOs.Customers;
using Firmness.Application.Interfaces;
using Firmness.Domain.Entities;
using Firmness.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Firmness.Infrastructure.Services.Identity;

/// <summary>
/// Implementation of IIdentityService using ASP.NET Core Identity
/// </summary>
public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<IdentityService> _logger;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        IMapper mapper,
        ILogger<IdentityService> logger)
    {
        _userManager = userManager;
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ResultOft<CustomerDto>> CreateUserAsync(CreateCustomerDto dto, string password)
    {
        try
        {
            var user = _mapper.Map<ApplicationUser>(dto);
            
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return ResultOft<CustomerDto>.Failure(errors);
            }
            
            // Assign default Customer role
            var roleResult = await _userManager.AddToRoleAsync(user, "Customer");

            if (!roleResult.Succeeded)
            {
                var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                return ResultOft<CustomerDto>.Failure(errors);
            }

            var customerDto = _mapper.Map<CustomerDto>(user);
            customerDto.Roles = new List<string> { "Customer" };
            
            _logger.LogInformation("User created: {UserId} - {UserName}", user.Id, user.UserName);

            return ResultOft<CustomerDto>.Success(customerDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return ResultOft<CustomerDto>.Failure("Error creating user. Please try again.");
        }
    }

    public async Task<ResultOft<CustomerDto>> GetUserByIdAsync(Guid userId)
    {
        try
        {
            if (userId == Guid.Empty)
                return ResultOft<CustomerDto>.Failure("Invalid user ID");

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return ResultOft<CustomerDto>.Failure($"User with ID {userId} not found");

            var roles = await _userManager.GetRolesAsync(user);
            var dto = _mapper.Map<CustomerDto>(user);
            dto.Roles = roles.ToList();

            return ResultOft<CustomerDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID {UserId}", userId);
            return ResultOft<CustomerDto>.Failure("Error loading user. Please try again.");
        }
    }

    public async Task<ResultOft<CustomerDto>> GetUserByEmailAsync(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                return ResultOft<CustomerDto>.Failure("Email is required");

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return ResultOft<CustomerDto>.Failure($"User with email {email} not found");

            var roles = await _userManager.GetRolesAsync(user);
            var dto = _mapper.Map<CustomerDto>(user);
            dto.Roles = roles.ToList();

            return ResultOft<CustomerDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email {Email}", email);
            return ResultOft<CustomerDto>.Failure("Error loading user. Please try again.");
        }
    }

    public async Task<ResultOft<CustomerDto>> UpdateUserAsync(Guid userId, UpdateCustomerDto dto)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return ResultOft<CustomerDto>.Failure($"User with ID {userId} not found");

            _mapper.Map(dto, user);
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                var errors = string.Join("; ", updateResult.Errors.Select(e => e.Description));
                return ResultOft<CustomerDto>.Failure(errors);
            }

            var customerDto = _mapper.Map<CustomerDto>(user);
            _logger.LogInformation("User updated: {UserId}", userId);

            return ResultOft<CustomerDto>.Success(customerDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", userId);
            return ResultOft<CustomerDto>.Failure("Error updating user. Please try again.");
        }
    }

    public async Task<Result> DeleteUserAsync(Guid userId)
    {
        try
        {
            if (userId == Guid.Empty)
                return Result.Failure("Invalid user ID");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Result.Failure($"User with ID {userId} not found");

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return Result.Failure(errors);
            }

            _logger.LogInformation("User deleted: {UserId}", userId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", userId);
            return Result.Failure("Error deleting user. Please try again.");
        }
    }

    public async Task<Result> ChangePasswordAsync(Guid userId, string newPassword)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Result.Failure($"User with ID {userId} not found");

            var removeResult = await _userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
                return Result.Failure("Error removing old password");

            var addResult = await _userManager.AddPasswordAsync(user, newPassword);
            if (!addResult.Succeeded)
            {
                var errors = string.Join("; ", addResult.Errors.Select(e => e.Description));
                return Result.Failure(errors);
            }

            _logger.LogInformation("Password changed for user: {UserId}", userId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user {UserId}", userId);
            return Result.Failure("Error changing password. Please try again.");
        }
    }

    public async Task<IList<string>> GetUserRolesAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            _logger.LogWarning("Attempt to get roles for non-existent user {UserId}", userId);
            throw new Exception("User not found");
        }
        
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<Result> AssignRoleAsync(Guid userId, string role)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Result.Failure($"User with ID {userId} not found");

            // Remove all current roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                    return Result.Failure("Error removing current roles");
            }

            // Add new role
            var addResult = await _userManager.AddToRoleAsync(user, role);
            if (!addResult.Succeeded)
            {
                var errors = string.Join("; ", addResult.Errors.Select(e => e.Description));
                return Result.Failure(errors);
            }

            _logger.LogInformation("Role {Role} assigned to user {UserId}", role, userId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role to user {UserId}", userId);
            return Result.Failure("Error assigning role. Please try again.");
        }
    }

    public async Task<Result> RemoveRoleAsync(Guid userId, string role)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Result.Failure($"User with ID {userId} not found");

            var result = await _userManager.RemoveFromRoleAsync(user, role);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return Result.Failure(errors);
            }

            _logger.LogInformation("Role {Role} removed from user {UserId}", role, userId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role from user {UserId}", userId);
            return Result.Failure("Error removing role. Please try again.");
        }
    }

    public async Task<ResultOft<IEnumerable<CustomerDto>>> GetCustomerUsersAsync(int page, int pageSize)
    {
        try
        {
            // ✅ OPTIMIZED: Single query with roles loaded efficiently
            var customerRoleName = "Customer";
            
            var query = from user in _context.Users
                        join userRole in _context.UserRoles on user.Id equals userRole.UserId
                        join role in _context.Roles on userRole.RoleId equals role.Id
                        where role.Name == customerRoleName
                        select user;

            var pagedUsers = await query
                .Distinct()
                .OrderBy(u => u.UserName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!pagedUsers.Any())
                return ResultOft<IEnumerable<CustomerDto>>.Failure("No customers found on this page.");

            // Load roles for all users in single query
            var userIds = pagedUsers.Select(u => u.Id).ToList();
            var userRolesDict = await _context.UserRoles
                .Where(ur => userIds.Contains(ur.UserId))
                .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
                .GroupBy(x => x.UserId)
                .ToDictionaryAsync(g => g.Key, g => g.Select(x => x.Name).ToList());

            var customerDtos = pagedUsers.Select(user =>
            {
                var dto = _mapper.Map<CustomerDto>(user);
                dto.Roles = userRolesDict.TryGetValue(user.Id, out var roles) ? roles : new List<string>();
                return dto;
            }).ToList();

            _logger.LogInformation("Retrieved {Count} customer users (page {Page})", customerDtos.Count, page);
            return ResultOft<IEnumerable<CustomerDto>>.Success(customerDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer users");
            return ResultOft<IEnumerable<CustomerDto>>.Failure("Error loading customers. Please try again.");
        }
    }

    public async Task<ResultOft<IEnumerable<CustomerDto>>> SearchUsersAsync(string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return ResultOft<IEnumerable<CustomerDto>>.Failure("Search term cannot be empty");

            if (searchTerm.Length < 2)
                return ResultOft<IEnumerable<CustomerDto>>.Failure("Search term must be at least 2 characters");

            var users = await _userManager.Users
                .Where(u => u.UserName.Contains(searchTerm) || u.FullName.Contains(searchTerm))
                .Take(50) // Safety limit
                .ToListAsync();

            var customerDtos = new List<CustomerDto>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var dto = _mapper.Map<CustomerDto>(user);
                dto.Roles = roles.ToList();
                customerDtos.Add(dto);
            }

            _logger.LogInformation("Search '{SearchTerm}' returned {Count} users", searchTerm, customerDtos.Count);
            return ResultOft<IEnumerable<CustomerDto>>.Success(customerDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with term '{SearchTerm}'", searchTerm);
            return ResultOft<IEnumerable<CustomerDto>>.Failure("Error searching users. Please try again.");
        }
    }

    public async Task<bool> UserExistsAsync(Guid userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user exists {UserId}", userId);
            return false;
        }
    }
}
