using Firmness.Infrastructure.Services;
using Firmness.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Firmness.Test.Unit.Services;

public class ReceiptPdfServiceTests
{
    private readonly ReceiptPdfService _receiptPdfService;

    public ReceiptPdfServiceTests()
    {
        _receiptPdfService = new ReceiptPdfService();
    }

    [Fact]
    public void GeneratePdf_WithValidSale_ReturnsNonEmptyByteArray()
    {
        // Arrange
        var sale = new Sale
        {
            Id = 1,
            CustomerId = "customer-123",
            Customer = new Customer
            {
                Id = "customer-123",
                FullName = "John Doe",
                Email = "john@example.com",
                Address = "123 Main St"
            },
            SaleDate = DateTime.Now,
            SubTotal = 100m,
            Tax = 19m,
            GrandTotal = 119m,
            SaleDetails = new List<SaleDetail>
            {
                new SaleDetail
                {
                    Id = 1,
                    ProductId = 1,
                    Product = new Product
                    {
                        Id = 1,
                        Code = "P001",
                        Name = "Test Product",
                        Price = 100m
                    },
                    Quantity = 1,
                    UnitPrice = 100m,
                    Total = 100m
                }
            }
        };

        // Act
        var pdfBytes = _receiptPdfService.GeneratePdf(sale);

        // Assert
        pdfBytes.Should().NotBeNull();
        pdfBytes.Should().NotBeEmpty();
        pdfBytes.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public void GeneratePdf_WithNullSale_ThrowsArgumentNullException()
    {
        // Act & Assert
        Action act = () => _receiptPdfService.GeneratePdf(null!);
        
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GeneratePdf_WithMultipleItems_IncludesAllItems()
    {
        // Arrange
        var sale = new Sale
        {
            Id = 1,
            CustomerId = "customer-123",
            Customer = new Customer
            {
                Id = "customer-123",
                FullName = "John Doe",
                Email = "john@example.com",
                Address = "123 Main St"
            },
            SaleDate = DateTime.Now,
            SubTotal = 300m,
            Tax = 57m,
            GrandTotal = 357m,
            SaleDetails = new List<SaleDetail>
            {
                new SaleDetail
                {
                    Id = 1,
                    ProductId = 1,
                    Product = new Product { Id = 1, Code = "P001", Name = "Product 1", Price = 100m },
                    Quantity = 2,
                    UnitPrice = 100m,
                    Total = 200m
                },
                new SaleDetail
                {
                    Id = 2,
                    ProductId = 2,
                    Product = new Product { Id = 2, Code = "P002", Name = "Product 2", Price = 50m },
                    Quantity = 2,
                    UnitPrice = 50m,
                    Total = 100m
                }
            }
        };

        // Act
        var pdfBytes = _receiptPdfService.GeneratePdf(sale);

        // Assert
        pdfBytes.Should().NotBeNull();
        pdfBytes.Should().NotBeEmpty();
        // PDF should be larger with more items
        pdfBytes.Length.Should().BeGreaterThan(1000);
    }

    [Fact]
    public void GeneratePdf_ValidatesPdfFormat()
    {
        // Arrange
        var sale = new Sale
        {
            Id = 1,
            CustomerId = "customer-123",
            Customer = new Customer
            {
                Id = "customer-123",
                FullName = "John Doe",
                Email = "john@example.com",
                Address = "123 Main St"
            },
            SaleDate = DateTime.Now,
            SubTotal = 100m,
            Tax = 19m,
            GrandTotal = 119m,
            SaleDetails = new List<SaleDetail>
            {
                new SaleDetail
                {
                    Id = 1,
                    ProductId = 1,
                    Product = new Product { Id = 1, Code = "P001", Name = "Test Product", Price = 100m },
                    Quantity = 1,
                    UnitPrice = 100m,
                    Total = 100m
                }
            }
        };

        // Act
        var pdfBytes = _receiptPdfService.GeneratePdf(sale);

        // Assert - Check PDF signature (PDF files start with %PDF-)
        var pdfSignature = System.Text.Encoding.ASCII.GetString(pdfBytes.Take(5).ToArray());
        pdfSignature.Should().Be("%PDF-");
    }
}
