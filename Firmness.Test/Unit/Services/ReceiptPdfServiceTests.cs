using Firmness.Application.Services;
using Firmness.Domain.Entities;
using FluentAssertions;
using Xunit;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO;

namespace Firmness.Test.Unit.Services;

public class ReceiptPdfServiceTests
{
    private readonly ReceiptPdfService _receiptPdfService;
    private readonly Mock<ILogger<ReceiptPdfService>> _loggerMock;

    public ReceiptPdfServiceTests()
    {
        _loggerMock = new Mock<ILogger<ReceiptPdfService>>();
        _receiptPdfService = new ReceiptPdfService(_loggerMock.Object);
        
        // Setup QuestPDF license for tests
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
    }

    [Fact]
    public async Task GeneratePdfAsync_WithValidSale_CreatesPdfFile()
    {
        // Arrange
        var outputPath = Path.Combine(Path.GetTempPath(), $"receipt_{Guid.NewGuid()}.pdf");
        var sale = new Sale
        {
            Id = 1,
            CustomerId = "customer-123",
            Customer = new Customer
            {
                Id = "customer-123",
                FullName = "John Doe",
                Email = "john@example.com",
            },
            Date = DateTime.Now,
            TotalAmount = 100m,
            TaxAmount = 19m,
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
                    UnitPrice = 100m
                }
            },
            Receipt = new Receipt { ReceiptNumber = "REC-001" }
        };

        try
        {
            // Act
            var result = await _receiptPdfService.GeneratePdfAsync(sale, outputPath);

            // Assert
            result.Should().BeTrue();
            File.Exists(outputPath).Should().BeTrue();
            
            var fileInfo = new FileInfo(outputPath);
            fileInfo.Length.Should().BeGreaterThan(0);
        }
        finally
        {
            // Cleanup
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
        }
    }

    [Fact]
    public async Task GeneratePdfAsync_WithInvalidPath_ReturnsFalse()
    {
        // Arrange
        // Use an invalid path (e.g., a directory that doesn't exist on a read-only location, or illegal chars)
        // On Linux/Unix, it's harder to find a truly "invalid" path structure that isn't just a permissions issue.
        // We'll try to write to a path that is clearly invalid or likely to fail if the directory creation fails.
        // However, the service creates directories. Let's try passing an empty string or null, but the signature expects string.
        
        // A better test might be to mock the file system, but we are testing the service directly.
        // Let's rely on the try-catch block in the service.
        
        var sale = new Sale();
        string outputPath = "/root/invalid/path/receipt.pdf"; // Likely permission denied or invalid

        // Act
        // Note: This test depends on the environment. If running as root, it might succeed.
        // Ideally we'd mock the file system interactions, but for now let's assume standard permissions.
        
        // If we can't guarantee failure, we should at least check that it handles exceptions gracefully.
        // But since we can't easily force an exception without mocking File.WriteAllBytes (which QuestPDF uses internally),
        // we might skip a "failure" test or try to pass a path that is a directory.
        
        var directoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(directoryPath);
        
        // Passing a directory path as a file path usually fails
        var result = await _receiptPdfService.GeneratePdfAsync(sale, directoryPath);

        // Assert
        result.Should().BeFalse();
        
        Directory.Delete(directoryPath);
    }
}
