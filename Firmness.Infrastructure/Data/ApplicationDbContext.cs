namespace Firmness.Infrastructure.Data;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Firmness.Domain.Entities;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<LogActivity> LogActivities { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleDetail> SaleDetails { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Receipt> Receipts { get; set; }

    // Configuring relationships and foreign keys
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Relationship between Sales and Customer
        builder.Entity<Sale>()
            .HasOne(s => s.Customer)
            .WithMany(c => c.Sales)
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);  // It prevents the cascading elimination of customers if there are associated sales.

        // Relationship between Sale and SaleDetail (one to many)
        builder.Entity<SaleDetail>()
            .HasOne(sd => sd.Sale)
            .WithMany(s => s.SaleDetails)
            .HasForeignKey(sd => sd.SaleId)
            .OnDelete(DeleteBehavior.Cascade);  // The sales details are removed when the sale is deleted.

        // Relationship between Receipt and Sale (one to one)
        builder.Entity<Receipt>()
            .HasOne(r => r.Sale)
            .WithOne(s => s.Receipt)
            .HasForeignKey<Receipt>(r => r.SaleId)
            .OnDelete(DeleteBehavior.Cascade);  // The receipt is deleted when the sale is deleted.

        // Relationship between SaleDetail and Product
        builder.Entity<SaleDetail>()
            .HasOne(sd => sd.Product)
            .WithMany()
            .HasForeignKey(sd => sd.ProductId)
            .OnDelete(DeleteBehavior.Restrict);  // It prevents the cascading deletion of products if there are associated sales details.
        
        builder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);  // If the category is deleted, the products become uncategorized.
    }
}
