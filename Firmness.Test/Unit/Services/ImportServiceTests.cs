using Firmness.Infrastructure.Services;
using Firmness.Application.Interfaces;
using Firmness.Application.DTOs.Excel;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using Xunit;
using Microsoft.AspNetCore.Http;

namespace Firmness.Test.Unit.Services;

/// <summary>
/// Unit tests for ImportService to ensure proper handling of Excel import operations
/// </summary>
public class ImportServiceTests
{
    private readonly Mock<IExcelService> _mockExcelService;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<IGenericRepository<Product>> _mockProductRepo;
    private readonly Mock<ILogger<ImportService>> _mockLogger;
    private readonly ImportService _sut; // System Under Test

    public ImportServiceTests()
    {
        _mockExcelService = new Mock<IExcelService>();
        _mockUserManager = MockUserManager<ApplicationUser>();
        _mockProductRepo = new Mock<IGenericRepository<Product>>();
        _mockLogger = new Mock<ILogger<ImportService>>();

        _sut = new ImportService(
            _mockExcelService.Object,
            _mockUserManager.Object,
            _mockProductRepo.Object,
            _mockLogger.Object
        );
    }

    #region ProcessExcelPreviewAsync Tests

    [Fact]
    public async Task ProcessExcelPreviewAsync_WithValidFile_ReturnsSuccessWithPreview()
    {
        // Arrange
        var mockFile = CreateMockFile("test.xlsx", 1024);
        var entityType = "customer";
        
        var headersResponse = new ExcelHeadersResponseDto
        {
            OriginalHeaders = new List<string> { "Username", "Email", "FullName" },
            CorrectHeaders = new List<string> { "Username", "Email", "FullName" }
        };
        
        var correctionResponse = new ExcelHeadersResponseDto
        {
            CorrectedColumns = new List<string> { "Username", "Email", "FullName" },
            WasCorrected = false
        };
        
        var preview = new BulkPreviewResultDto
        {
            ValidRows = new List<RowValidationResultDto>
            {
                new() { RowNumber = 2, IsValid = true, RowData = new Dictionary<string, string>() }
            },
            InvalidRows = new List<RowValidationResultDto>()
        };

        _mockExcelService
            .Setup(x => x.ExtractHeadersFromExcelAsync(It.IsAny<IFormFile>(), entityType))
            .ReturnsAsync(ResultOft<ExcelHeadersResponseDto>.Success(headersResponse));
        
        _mockExcelService
            .Setup(x => x.CorrectColumnNamesAsync(It.IsAny<List<string>>(), It.IsAny<List<string>>()))
            .ReturnsAsync(ResultOft<ExcelHeadersResponseDto>.Success(correctionResponse));
        
        _mockExcelService
            .Setup(x => x.GeneratePreviewAsync(It.IsAny<Stream>(), entityType, It.IsAny<List<string>>()))
            .ReturnsAsync(preview);

        // Act
        var result = await _sut.ProcessExcelPreviewAsync(mockFile, entityType);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.TotalValid.Should().Be(1);
        result.Data.TotalInvalid.Should().Be(0);
    }

    [Fact]
    public async Task ProcessExcelPreviewAsync_WithEmptyFile_ReturnsFailure()
    {
        // Arrange
        IFormFile? nullFile = null;
        var entityType = "customer";

        // Act
        var result = await _sut.ProcessExcelPreviewAsync(nullFile!, entityType);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("vacío");
    }

    [Fact]
    public async Task ProcessExcelPreviewAsync_WithInvalidHeaders_ReturnsFailure()
    {
        // Arrange
        var mockFile = CreateMockFile("test.xlsx", 1024);
        var entityType = "customer";
        
        _mockExcelService
            .Setup(x => x.ExtractHeadersFromExcelAsync(It.IsAny<IFormFile>(), entityType))
            .ReturnsAsync(ResultOft<ExcelHeadersResponseDto>.Failure("Invalid headers"));

        // Act
        var result = await _sut.ProcessExcelPreviewAsync(mockFile, entityType);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Invalid headers");
    }

    #endregion

    #region ConfirmImportAsync Tests

    [Fact]
    public async Task ConfirmImportAsync_WithEmptyValidRows_ReturnsSuccessWithZeroInserted()
    {
        // Arrange
        var preview = new BulkPreviewResultDto
        {
            ValidRows = new List<RowValidationResultDto>(),
            InvalidRows = new List<RowValidationResultDto>()
        };

        // Act
        var result = await _sut.ConfirmImportAsync(preview);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.InsertedCount.Should().Be(0);
        result.Data.FailedRows.Should().BeEmpty();
    }

    [Fact]
    public async Task ConfirmImportAsync_WithValidCustomers_CallsUserManager()
    {
        // Arrange
        var preview = new BulkPreviewResultDto
        {
            ValidRows = new List<RowValidationResultDto>
            {
                new()
                {
                    RowNumber = 2,
                    IsValid = true,
                    RowData = new Dictionary<string, string>
                    {
                        { "Username", "testuser" },
                        { "FullName", "Test User" },
                        { "Email", "test@example.com" },
                        { "Address", "123 Street" },
                        { "Phone", "123456789" }
                    }
                }
            }
        };

        _mockUserManager
            .Setup(x => x.CreateAsync(It.IsAny<Customer>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        
        _mockUserManager
            .Setup(x => x.AddToRoleAsync(It.IsAny<Customer>(), "Customer"))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _sut.ConfirmImportAsync(preview);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.InsertedCount.Should().Be(1);
        _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<Customer>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ConfirmImportAsync_WithValidProducts_CallsRepository()
    {
        // Arrange
        var preview = new BulkPreviewResultDto
        {
            ValidRows = new List<RowValidationResultDto>
            {
                new()
                {
                    RowNumber = 2,
                    IsValid = true,
                    RowData = new Dictionary<string, string>
                    {
                        { "Name", "Test Product" },
                        { "Price", "100" },
                        { "CategoryId", "1" },
                        { "Stock", "50" }
                    }
                }
            }
        };

        _mockProductRepo
            .Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<Product>>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.ConfirmImportAsync(preview);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.InsertedCount.Should().Be(1);
        _mockProductRepo.Verify(x => x.AddRangeAsync(It.IsAny<IEnumerable<Product>>()), Times.Once);
    }

    #endregion

    #region Helper Methods

    private static IFormFile CreateMockFile(string fileName, long length)
    {
        var mockFile = new Mock<IFormFile>();
        var content = "Fake file content";
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;

        mockFile.Setup(f => f.FileName).Returns(fileName);
        mockFile.Setup(f => f.Length).Returns(length);
        mockFile.Setup(f => f.OpenReadStream()).Returns(ms);
        mockFile.Setup(f => f.ContentDisposition).Returns($"form-data; name=\"file\"; filename=\"{fileName}\"");

        return mockFile.Object;
    }

    private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        var mgr = new Mock<UserManager<TUser>>(
            store.Object, null, null, null, null, null, null, null, null);
        mgr.Object.UserValidators.Add(new UserValidator<TUser>());
        mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());
        return mgr;
    }

    #endregion
}
