namespace Firmness.Application.Services.Customers;

using Firmness.Application.Interfaces;
using Microsoft.Extensions.Logging;

/// <summary>
/// Service for handling Customer role  management operations using IIdentityService abstraction
/// </summary>
public class CustomerRoleManagementService : ICustomerRoleManagementService
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<CustomerRoleManagementService> _logger;

    public CustomerRoleManagementService(
        IIdentityService identityService,
        ILogger<CustomerRoleManagementService> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId)
    {
        return await _identityService.GetUserRolesAsync(userId);
    }

    public async Task<IEnumerable<string>> GetAllRolesAsync()
    {
        // TODO: Obtener dinámicamente de RoleManager si es necesario
        return new List<string> { "Admin", "Customer" };
    }

    public async Task UpdateUserRoleAsync(Guid userId, string newRole)
    {
        var result = await _identityService.AssignRoleAsync(userId, newRole);
        
        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to update role for user {UserId}: {Error}", userId, result.ErrorMessage);
            throw new Exception(result.ErrorMessage);
        }
        
        _logger.LogInformation("User {UserId} role changed to {NewRole}", userId, newRole);
    }
}
