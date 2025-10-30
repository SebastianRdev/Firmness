namespace Firmness.Infrastructure.Data;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Firmness.Infrastructure.Data.Entities;
using Firmness.Infrastructure.Data.Identity;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<LogActivity> LogActivities { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }

}