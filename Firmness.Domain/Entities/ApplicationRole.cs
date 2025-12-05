namespace Firmness.Domain.Entities;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents a role in the application, extending IdentityRole.
/// </summary>
public class ApplicationRole : IdentityRole
{
    /// <summary>
    /// Gets or sets the description of the role.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationRole"/> class.
    /// </summary>
    public ApplicationRole() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationRole"/> class with a specified name and description.
    /// </summary>
    /// <param name="roleName">The name of the role.</param>
    /// <param name="description">The description of the role.</param>
    public ApplicationRole(string roleName, string? description = null) : base(roleName)
    {
        Description = description;
    }
}