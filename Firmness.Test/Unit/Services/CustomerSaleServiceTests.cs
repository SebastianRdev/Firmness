using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Firmness.Application.Services;
using Firmness.Application.DTOs.Sales;
using Firmness.Application.Interfaces;
using Firmness.Application.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using Moq;
using FluentAssertions;
using Xunit;
using System.Linq.Expressions;

namespace Firmness.Test.Unit.Services;

public class CustomerSaleServiceTests
{
    private readonly Mock<IGenericRepository<Customer>> _customerRepositoryMock;
    private readonly Mock<IGenericRepository<Sale>> _saleRepositoryMock;
    private readonly Mock<IGenericRepository<Product>> _productRepositoryMock;
    private readonly Mock<IGenericRepository<Receipt>> _receiptRepositoryMock;
    private readonly Mock<IReceiptPdfService> _pdfServiceMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IWebHostEnvironment> _environmentMock;
    private readonly Mock<IOptions<EmailSettings>> _emailSettingsMock;
    private readonly Mock<ILogger<CustomerSaleService>> _loggerMock;
    private readonly CustomerSaleService _customerSaleService;

    public CustomerSaleServiceTests()
    {
        _customerRepositoryMock = new Mock<IGenericRepository<Customer>>();
        _saleRepositoryMock = new Mock<IGenericRepository<Sale>>();
        _productRepositoryMock = new Mock<IGenericRepository<Product>>();
        _receiptRepositoryMock = new Mock<IGenericRepository<Receipt>>();
        _pdfServiceMock = new Mock<IReceiptPdfService>();
        _emailServiceMock = new Mock<IEmailService>();
        _environmentMock = new Mock<IWebHostEnvironment>();
        _emailSettingsMock = new Mock<IOptions<EmailSettings>>();
        _loggerMock = new Mock<ILogger<CustomerSaleService>>();

        _emailSettingsMock.Setup(x => x.Value).Returns(new EmailSettings());
        _environmentMock.Setup(x => x.WebRootPath).Returns("wwwroot");

        _customerSaleService = new CustomerSaleService(
            _customerRepositoryMock.Object,
            _productRepositoryMock.Object,
            _saleRepositoryMock.Object,
            _receiptRepositoryMock.Object,
            _pdfServiceMock.Object,
            _emailServiceMock.Object,
            _environmentMock.Object,
            _emailSettingsMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task CreateSaleWithReceiptAsync_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var customerIdStr = customerId.ToString();
        var customer = new Customer { Id = customerIdStr, Email = "test@example.com", FullName = "Test User" };
        var product = new Product { Id = 1, Code = "P001", Name = "Product 1", Price = 100m, Stock = 10 };

        _customerRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<Expression<Func<Customer, object>>[]>()))
            .ReturnsAsync(customer);

        _productRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<Expression<Func<Product, object>>[]>()))
            .ReturnsAsync(product);

        var saleDto = new CreateSaleDto
        {
            CustomerId = customerId,
            Date = DateTime.Now,
            TotalAmount = 200m,
            TaxAmount = 38m,
            GrandTotal = 238m,
            SaleDetails = new List<CreateSaleDetailDto>
            {
                new CreateSaleDetailDto { ProductId = 1, Quantity = 2, UnitPrice = 100m }
            }
        };

        _saleRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Sale>()))
            .ReturnsAsync((Sale s) => { s.Id = 1; return s; });
            
        _receiptRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Receipt>()))
            .ReturnsAsync((Receipt r) => { r.Id = 1; return r; });

        _pdfServiceMock.Setup(s => s.GeneratePdfAsync(It.IsAny<Sale>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var result = await _customerSaleService.CreateSaleWithReceiptAsync(saleDto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(1);
        result.Data.GrandTotal.Should().Be(238m);
        
        _productRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Product>(p => p.Stock == 8)), Times.Once);
        _emailServiceMock.Verify(e => e.SendEmailToMultipleAsync(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task CreateSaleWithReceiptAsync_WithInsufficientStock_ReturnsFailure()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var customerIdStr = customerId.ToString();
        var customer = new Customer { Id = customerIdStr, Email = "test@example.com", FullName = "Test User" };
        var product = new Product { Id = 1, Code = "P001", Name = "Product 1", Price = 100m, Stock = 1 };

        _customerRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<Expression<Func<Customer, object>>[]>()))
            .ReturnsAsync(customer);

        _productRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<Expression<Func<Product, object>>[]>()))
            .ReturnsAsync(product);

        var saleDto = new CreateSaleDto
        {
            CustomerId = customerId,
            SaleDetails = new List<CreateSaleDetailDto>
            {
                new CreateSaleDetailDto { ProductId = 1, Quantity = 2, UnitPrice = 100m }
            }
        };

        // Act
        var result = await _customerSaleService.CreateSaleWithReceiptAsync(saleDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Insufficient stock");
    }

    [Fact]
    public async Task GetSaleByIdAsync_WithValidId_ReturnsSale()
    {
        // Arrange
        var saleId = 1;
        var customerId = Guid.NewGuid();
        var expectedSale = new Sale
        {
            Id = saleId,
            CustomerId = customerId.ToString(),
            Customer = new Customer { Id = customerId.ToString(), FullName = "Test User", Email = "test@example.com" },
            TotalAmount = 100m,
            TaxAmount = 19m,
            GrandTotal = 119m,
            SaleDetails = new List<SaleDetail>(),
            Receipt = new Receipt { FileName = "test.pdf" }
        };

        _saleRepositoryMock.Setup(r => r.GetByIdAsync(saleId, It.IsAny<Expression<Func<Sale, object>>[]>()))
            .ReturnsAsync(expectedSale);

        // Act
        var result = await _customerSaleService.GetSaleByIdAsync(saleId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(saleId);
    }
}
