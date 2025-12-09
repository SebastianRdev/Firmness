using Firmness.Application.Common;
using Firmness.Application.DTOs.Customers;
using Firmness.Domain.Entities;

namespace Firmness.Application.Interfaces;

/// <summary>
/// Service interface for Identity management operations, abstracting UserManager dependency
/// </summary>
public interface IIdentityService
{
    /// <summary>
    /// Creates a new user with password
    /// </summary>
    Task<ResultOft<CustomerDto>> CreateUserAsync(CreateCustomerDto dto, string password);
    
    /// <summary>
    /// Gets user by ID
    /// </summary>
    Task<ResultOft<CustomerDto>> GetUserByIdAsync(Guid userId);
    
    /// <summary>
    /// Gets user by email
    /// </summary>
    Task<ResultOft<CustomerDto>> GetUserByEmailAsync(string email);
    
    /// <summary>
    /// Updates user information
    /// </summary>
    Task<ResultOft<CustomerDto>> UpdateUserAsync(Guid userId, UpdateCustomerDto dto);
    
    /// <summary>
    /// Deletes a user
    /// </summary>
    Task<Result> DeleteUserAsync(Guid userId);
    
    /// <summary>
    /// Changes user password
    /// </summary>
    Task<Result> ChangePasswordAsync(Guid userId, string newPassword);
    
    /// <summary>
    /// Gets user roles
    /// </summary>
    Task<IList<string>> GetUserRolesAsync(Guid userId);
    
    /// <summary>
    /// Assigns a role to a user
    /// </summary>
    Task<Result> AssignRoleAsync(Guid userId, string role);
    
    /// <summary>
    /// Removes a role from a user
    /// </summary>
    Task<Result> RemoveRoleAsync(Guid userId, string role);
    
    /// <summary>
    /// Gets all users with Customer role (paginated, with roles loaded)
    /// </summary>
    Task<ResultOft<IEnumerable<CustomerDto>>> GetCustomerUsersAsync(int page, int pageSize);
    
    /// <summary>
    /// Searches users by username or full name
    /// </summary>
    Task<ResultOft<IEnumerable<CustomerDto>>> SearchUsersAsync(string searchTerm);
    
    /// <summary>
    /// Checks if user exists
    /// </summary>
    Task<bool> UserExistsAsync(Guid userId);
}
