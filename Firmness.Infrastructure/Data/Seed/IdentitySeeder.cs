using Microsoft.AspNetCore.Identity;
using Firmness.Domain.Entities;

namespace Firmness.Infrastructure.Data.Seed;

public static class IdentitySeeder
{
    public static async Task SeedAsync(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        string[] roles = { "Admin", "Customer" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = role });
            }
        }
    }
}