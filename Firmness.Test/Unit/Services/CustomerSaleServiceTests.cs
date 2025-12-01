using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Firmness.Application.Services;
using Firmness.Application.DTOs.Sale;
using Moq;
using FluentAssertions;
using Xunit;

namespace Firmness.Test.Unit.Services;

public class CustomerSaleServiceTests
{
    private readonly Mock<IGenericRepository<Sale>> _saleRepositoryMock;
    private readonly Mock<IGenericRepository<Product>> _productRepositoryMock;
    private readonly Mock<IGenericRepository<Receipt>> _receiptRepositoryMock;
    private readonly CustomerSaleService _customerSaleService;

    public CustomerSaleServiceTests()
    {
        _saleRepositoryMock = new Mock<IGenericRepository<Sale>>();
        _productRepositoryMock = new Mock<IGenericRepository<Product>>();
        _receiptRepositoryMock = new Mock<IGenericRepository<Receipt>>();
        
        _customerSaleService = new CustomerSaleService(
            _saleRepositoryMock.Object,
            _productRepositoryMock.Object,
            _receiptRepositoryMock.Object
        );
    }

    [Fact]
    public async Task CreateSale_WithValidData_CalculatesTotalsCorrectly()
    {
        // Arrange
        var customerId = "customer-123";
        var product1 = new Product { Id = 1, Code = "P001", Name = "Product 1", Price = 100m, Stock = 10 };
        var product2 = new Product { Id = 2, Code = "P002", Name = "Product 2", Price = 50m, Stock = 5 };

        _productRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product1);
        _productRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(product2);

        var saleDto = new CreateSaleDto
        {
            CustomerId = customerId,
            Details = new List<SaleDetailDto>
            {
                new SaleDetailDto { ProductId = 1, Quantity = 2 }, // 2 * 100 = 200
                new SaleDetailDto { ProductId = 2, Quantity = 3 }  // 3 * 50 = 150
            }
        };

        Sale? capturedSale = null;
        _saleRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Sale>()))
            .Callback<Sale>(s => capturedSale = s)
            .ReturnsAsync((Sale s) => s);

        // Act
        var result = await _customerSaleService.CreateSaleAsync(saleDto);

        // Assert
        result.Should().NotBeNull();
        capturedSale.Should().NotBeNull();
        capturedSale!.SubTotal.Should().Be(350m); // 200 + 150
        capturedSale.Tax.Should().Be(66.5m); // 350 * 0.19
        capturedSale.GrandTotal.Should().Be(416.5m); // 350 + 66.5
    }

    [Fact]
    public async Task CreateSale_WithValidData_UpdatesProductStock()
    {
        // Arrange
        var customerId = "customer-123";
        var product = new Product { Id = 1, Code = "P001", Name = "Product 1", Price = 100m, Stock = 10 };

        _productRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        var saleDto = new CreateSaleDto
        {
            CustomerId = customerId,
            Details = new List<SaleDetailDto>
            {
                new SaleDetailDto { ProductId = 1, Quantity = 3 }
            }
        };

        _saleRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Sale>()))
            .ReturnsAsync((Sale s) => s);

        // Act
        await _customerSaleService.CreateSaleAsync(saleDto);

        // Assert
        product.Stock.Should().Be(7); // 10 - 3
        _productRepositoryMock.Verify(r => r.UpdateAsync(product), Times.Once);
    }

    [Fact]
    public async Task CreateSale_WithInsufficientStock_ThrowsException()
    {
        // Arrange
        var customerId = "customer-123";
        var product = new Product { Id = 1, Code = "P001", Name = "Product 1", Price = 100m, Stock = 2 };

        _productRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        var saleDto = new CreateSaleDto
        {
            CustomerId = customerId,
            Details = new List<SaleDetailDto>
            {
                new SaleDetailDto { ProductId = 1, Quantity = 5 } // More than available stock
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _customerSaleService.CreateSaleAsync(saleDto)
        );
    }

    [Fact]
    public async Task GetSaleById_WithValidId_ReturnsSale()
    {
        // Arrange
        var saleId = 1;
        var expectedSale = new Sale
        {
            Id = saleId,
            CustomerId = "customer-123",
            SubTotal = 100m,
            Tax = 19m,
            GrandTotal = 119m
        };

        _saleRepositoryMock.Setup(r => r.GetByIdAsync(saleId)).ReturnsAsync(expectedSale);

        // Act
        var result = await _customerSaleService.GetSaleByIdAsync(saleId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedSale);
    }
}
