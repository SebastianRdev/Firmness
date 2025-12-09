namespace Firmness.Application.Services.Customers;

using Firmness.Application.Interfaces;
using Firmness.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

/// <summary>
/// Service for handling Customer role management operations
/// </summary>
public class CustomerRoleManagementService : ICustomerRoleManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<CustomerRoleManagementService> _logger;

    public CustomerRoleManagementService(
        UserManager<ApplicationUser> userManager,
        ILogger<CustomerRoleManagementService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            _logger.LogWarning("Attempt to get roles for non-existent user {UserId}", userId);
            throw new Exception("User not found");
        }
        
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<IEnumerable<string>> GetAllRolesAsync()
    {
        // TODO: Obtener dinámicamente de RoleManager si es necesario
        return new List<string> { "Admin", "Customer" };
    }

    public async Task UpdateUserRoleAsync(Guid userId, string newRole)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            _logger.LogWarning("Attempt to update role for non-existent user {UserId}", userId);
            throw new Exception("User not found");
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles.ToArray());
        await _userManager.AddToRoleAsync(user, newRole);
        
        _logger.LogInformation("User {UserId} role changed to {NewRole}", userId, newRole);
    }
}
