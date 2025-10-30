namespace Firmness.Infrastructure.Data.Identity;

using Microsoft.AspNetCore.Identity;
using Firmness.Infrastructure.Data.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relación con tus logs
    public List<LogActivity> Activities { get; set; } = new();
}
