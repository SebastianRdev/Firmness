namespace Firmness.Infrastructure.Data.Seed;

using Microsoft.AspNetCore.Identity;
using Domain.Entities;

public static class AdminSeed
{
    public static async Task SeedAdminsAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        var adminUsers = new[]
        {
            new ApplicationUser
            {
                UserName = "admin1",
                Email = "admin1@firmness.com",
                FullName = "System Admin One",
                EmailConfirmed = true
            },
            new ApplicationUser
            {
                UserName = "admin2",
                Email = "admin2@firmness.com",
                FullName = "System Admin Two",
                EmailConfirmed = true
            }
        };

        foreach (var admin in adminUsers)
        {
            var existingUser = await userManager.FindByEmailAsync(admin.Email);
            if (existingUser == null)
            {
                var result = await userManager.CreateAsync(admin, "Admin123$");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}