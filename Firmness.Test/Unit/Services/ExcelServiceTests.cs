using Firmness.Application.Services;
using Firmness.Application.Interfaces;
using Firmness.Application.DTOs.Excel;
using Firmness.Application.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using Xunit;
using OfficeOpenXml;

namespace Firmness.Test.Unit.Services;

/// <summary>
/// Unit tests for ExcelService to ensure proper Excel file operations
/// </summary>
public class ExcelServiceTests
{
    private readonly Mock<IGeminiService> _mockGeminiService;
    private readonly Mock<ILogger<ExcelService>> _mockLogger;
    private readonly ExcelService _sut;

    public ExcelServiceTests()
    {
        _mockGeminiService = new Mock<IGeminiService>();
        _mockLogger = new Mock<ILogger<ExcelService>>();
        _sut = new ExcelService(_mockGeminiService.Object, _mockLogger.Object);
        
        // Set EPPlus license
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    #region GetHeadersAsync Tests

    [Fact]
    public async Task GetHeadersAsync_WithValidExcel_ReturnsHeaders()
    {
        // Arrange
        var mockFile = CreateExcelFileWithHeaders(new[] { "Username", "Email", "FullName" });

        // Act
        var result = await _sut.GetHeadersAsync(mockFile);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(new[] { "Username", "Email", "FullName" });
    }

    [Fact]
    public async Task GetHeadersAsync_WithEmptyExcel_ReturnsEmptyList()
    {
        // Arrange
        var mockFile = CreateEmptyExcelFile();

        // Act
        var result = await _sut.GetHeadersAsync(mockFile);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    #endregion

    #region ExtractHeadersFromExcelAsync Tests

    [Fact]
    public async Task ExtractHeadersFromExcelAsync_WithValidFile_ReturnsSuccess()
    {
        // Arrange
        var mockFile = CreateExcelFileWithHeaders(new[] { "Username", "Email", "FullName" });
        var entityType = "customer";

        // Act
        var result = await _sut.ExtractHeadersFromExcelAsync(mockFile, entityType);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.OriginalHeaders.Should().HaveCount(3);
        result.Data.CorrectHeaders.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ExtractHeadersFromExcelAsync_WithNullFile_ReturnsFailure()
    {
        // Arrange
        IFormFile? nullFile = null;
        var entityType = "customer";

        // Act
        var result = await _sut.ExtractHeadersFromExcelAsync(nullFile!, entityType);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("empty");
    }

    [Fact]
    public async Task ExtractHeadersFromExcelAsync_WithUnsupportedEntityType_ReturnsFailure()
    {
        // Arrange
        var mockFile = CreateExcelFileWithHeaders(new[] { "Column1", "Column2" });
        var entityType = "unsupported";

        // Act
        var result = await _sut.ExtractHeadersFromExcelAsync(mockFile, entityType);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("template");
    }

    #endregion

    #region CorrectColumnNamesAsync Tests

    [Fact]
    public async Task CorrectColumnNamesAsync_WithGeminiSuccess_ReturnsCorrection()
    {
        // Arrange
        var realColumns = new List<string> { "user", "mail", "name" };
        var correctColumns = new List<string> { "Username", "Email", "FullName" };
        
        var expectedResponse = new ExcelHeadersResponseDto
        {
            CorrectedColumns = correctColumns,
            WasCorrected = true,
            ChangesReport = "Corrected 3 columns"
        };

        _mockGeminiService
            .Setup(x => x.CorrectColumnNamesAsync(realColumns, correctColumns))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _sut.CorrectColumnNamesAsync(realColumns, correctColumns);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.CorrectedColumns.Should().BeEquivalentTo(correctColumns);
        result.Data.WasCorrected.Should().BeTrue();
    }

    [Fact]
    public async Task CorrectColumnNamesAsync_WhenGeminiReturnsNull_ReturnsFailure()
    {
        // Arrange
        var realColumns = new List<string> { "col1", "col2" };
        var correctColumns = new List<string> { "Column1", "Column2" };

        _mockGeminiService
            .Setup(x => x.CorrectColumnNamesAsync(realColumns, correctColumns))
            .ReturnsAsync((ExcelHeadersResponseDto?)null);

        // Act
        var result = await _sut.CorrectColumnNamesAsync(realColumns, correctColumns);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("no data");
    }

    [Fact]
    public async Task CorrectColumnNamesAsync_WhenGeminiThrows_ReturnsFailure()
    {
        // Arrange
        var realColumns = new List<string> { "col1" };
        var correctColumns = new List<string> { "Column1" };

        _mockGeminiService
            .Setup(x => x.CorrectColumnNamesAsync(realColumns, correctColumns))
            .ThrowsAsync(new Exception("Gemini API error"));

        // Act
        var result = await _sut.CorrectColumnNamesAsync(realColumns, correctColumns);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Gemini");
    }

    #endregion

    #region GeneratePreviewAsync Tests

    [Fact]
    public async Task GeneratePreviewAsync_WithValidRows_ReturnsValidAndInvalidRows()
    {
        // Arrange
        var mockFile = CreateExcelFileWithData(
            headers: new[] { "Username", "Email", "FullName" },
            rows: new[]
            {
                new[] { "user1", "user1@test.com", "User One" },
                new[] { "", "invalid", "User Two" } // Invalid row (empty username)
            }
        );

        var correctedHeaders = new List<string> { "Username", "Email", "FullName" };
        var entityType = "customer";

        using var stream = mockFile.OpenReadStream();

        // Act
        var result = await _sut.GeneratePreviewAsync(stream, entityType, correctedHeaders);

        // Assert
        result.Should().NotBeNull();
        // Note: Actual validation depends on your CustomerRowValidator implementation
        // This is a basic structure test
        (result.TotalValid + result.TotalInvalid).Should().Be(2);
    }

    #endregion

    #region Helper Methods

    private static IFormFile CreateExcelFileWithHeaders(string[] headers)
    {
        var stream = new MemoryStream();
        using (var package = new ExcelPackage(stream))
        {
            var worksheet = package.Workbook.Worksheets.Add("Sheet1");
            
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
            }
            
            package.Save();
        }
        stream.Position = 0;

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("test.xlsx");
        mockFile.Setup(f => f.Length).Returns(stream.Length);
        mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
        mockFile.Setup(f => f.ContentType).Returns("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        return mockFile.Object;
    }

    private static IFormFile CreateExcelFileWithData(string[] headers, string[][] rows)
    {
        var stream = new MemoryStream();
        using (var package = new ExcelPackage(stream))
        {
            var worksheet = package.Workbook.Worksheets.Add("Sheet1");
            
            // Add headers
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
            }
            
            // Add data rows
            for (int rowIndex = 0; rowIndex < rows.Length; rowIndex++)
            {
                for (int colIndex = 0; colIndex < rows[rowIndex].Length; colIndex++)
                {
                    worksheet.Cells[rowIndex + 2, colIndex + 1].Value = rows[rowIndex][colIndex];
                }
            }
            
            package.Save();
        }
        stream.Position = 0;

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("test.xlsx");
        mockFile.Setup(f => f.Length).Returns(stream.Length);
        mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
        mockFile.Setup(f => f.ContentType).Returns("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        return mockFile.Object;
    }

    private static IFormFile CreateEmptyExcelFile()
    {
        var stream = new MemoryStream();
        using (var package = new ExcelPackage(stream))
        {
            package.Workbook.Worksheets.Add("Sheet1");
            package.Save();
        }
        stream.Position = 0;

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("empty.xlsx");
        mockFile.Setup(f => f.Length).Returns(stream.Length);
        mockFile.Setup(f => f.OpenReadStream()).Returns(stream);

        return mockFile.Object;
    }

    #endregion
}
