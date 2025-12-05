namespace Firmness.Domain.Entities;

using Microsoft.AspNetCore.Identity;
using Domain.Entities;

/// <summary>
/// Represents a user in the application, extending the IdentityUser.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Gets or sets the full name of the user.
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// Gets or sets the address of the user.
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the user was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the list of activities logged for the user.
    /// </summary>
    public List<LogActivity> Activities { get; set; } = new();
}
