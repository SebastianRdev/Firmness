namespace Firmness.Application.Interfaces;

/// <summary>
/// Service interface for Customer role management operations
/// </summary>
public interface ICustomerRoleManagementService
{
    /// <summary>
    /// Retrieves the roles assigned to a specific user
    /// </summary>
    Task<IEnumerable<string>> GetUserRolesAsync(Guid userId);
    
    /// <summary>
    /// Retrieves all available roles in the system
    /// </summary>
    Task<IEnumerable<string>> GetAllRolesAsync();
    
    /// <summary>
    /// Updates the role of a specific user
    /// </summary>
    Task UpdateUserRoleAsync(Guid userId, string newRole);
}
