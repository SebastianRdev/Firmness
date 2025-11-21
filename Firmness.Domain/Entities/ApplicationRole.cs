namespace Firmness.Domain.Entities;

using Microsoft.AspNetCore.Identity;

public class ApplicationRole : IdentityRole
{
    // Optional field: add properties if necessary
    public string? Description { get; set; }

    // Empty constructor (necessary for EF)
    public ApplicationRole() : base() { }

    // Constructor that allows you to initialize the role directly with name and description
    public ApplicationRole(string roleName, string? description = null) : base(roleName)
    {
        Description = description;
    }
}