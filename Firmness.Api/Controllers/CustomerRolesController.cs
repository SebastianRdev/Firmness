namespace Firmness.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Firmness.Application.Interfaces;

/// <summary>
/// Manages customer roles
/// </summary>
[ApiController]
[Route("api/customers/{customerId}/roles")]
// [Authorize(Roles = "Admin")]
public class CustomerRolesController : ControllerBase
{
    private readonly ICustomerRoleManagementService _customerRoleService;
    private readonly ILogger<CustomerRolesController> _logger;

    public CustomerRolesController(
        ICustomerRoleManagementService customerRoleService,
        ILogger<CustomerRolesController> logger)
    {
        _customerRoleService = customerRoleService;
        _logger = logger;
    }

    /// <summary>
    /// Gets the roles assigned to a specific customer
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetUserRoles(Guid customerId)
    {
        try
        {
            var roles = await _customerRoleService.GetUserRolesAsync(customerId);
            return Ok(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles for customer {CustomerId}", customerId);
            return NotFound(new { error = "Customer not found" });
        }
    }

    /// <summary>
    /// Updates the role of a specific customer
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> UpdateUserRole(Guid customerId, [FromBody] UpdateRoleRequest request)
    {
        try
        {
            await _customerRoleService.UpdateUserRoleAsync(customerId, request.Role);
            return Ok(new { message = "Role updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role for customer {CustomerId}", customerId);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Gets all available roles in the system
    /// </summary>
    [HttpGet("/api/roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        try
        {
            var roles = await _customerRoleService.GetAllRolesAsync();
            return Ok(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all roles");
            return StatusCode(500, new { error = "Error loading roles" });
        }
    }

    #region DTOs

    public record UpdateRoleRequest(string Role);

    #endregion
}
