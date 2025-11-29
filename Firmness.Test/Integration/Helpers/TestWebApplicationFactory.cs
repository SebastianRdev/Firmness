using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Firmness.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Firmness.Domain.Entities;

namespace Firmness.Test.Integration.Helpers;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add DbContext using an in-memory database for testing
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase");
            });

            // Build the service provider
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                var userManager = scopedServices.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scopedServices.GetRequiredService<RoleManager<ApplicationRole>>();

                // Ensure the database is created
                db.Database.EnsureCreated();

                // Seed test data
                SeedTestData(db, userManager, roleManager).Wait();
            }
        });
    }

    private async Task SeedTestData(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        // Create roles
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
        }

        if (!await roleManager.RoleExistsAsync("Customer"))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = "Customer" });
        }

        // Create test user
        var testUser = await userManager.FindByEmailAsync("test@example.com");
        if (testUser == null)
        {
            testUser = new ApplicationUser
            {
                UserName = "test@example.com",
                Email = "test@example.com",
                FullName = "Test User",
                Address = "Test Address",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(testUser, "Test123$");
            await userManager.AddToRoleAsync(testUser, "Customer");
        }

        // Seed test products
        if (!context.Products.Any())
        {
            var category = new Category
            {
                Name = "Test Category",
                Description = "Test Description"
            };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            context.Products.AddRange(
                new Product
                {
                    Code = "TEST001",
                    Name = "Test Product 1",
                    Description = "Test Description 1",
                    Price = 100m,
                    Stock = 10,
                    CategoryId = category.Id
                },
                new Product
                {
                    Code = "TEST002",
                    Name = "Test Product 2",
                    Description = "Test Description 2",
                    Price = 50m,
                    Stock = 5,
                    CategoryId = category.Id
                }
            );

            await context.SaveChangesAsync();
        }
    }
}
