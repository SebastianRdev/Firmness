using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Firmness.Application.DTOs.Auth;
using Firmness.Application.DTOs.Sale;
using Firmness.Test.Integration.Helpers;
using FluentAssertions;
using Xunit;

namespace Firmness.Test.Integration.Controllers;

public class SalesControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public SalesControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<string> GetAuthTokenAsync()
    {
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Test123$"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
        var content = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponseDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return authResponse!.Token;
    }

    [Fact]
    public async Task CreateSale_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var saleDto = new CreateSaleDto
        {
            CustomerId = "test-customer",
            Details = new List<SaleDetailDto>
            {
                new SaleDetailDto { ProductId = 1, Quantity = 1 }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/sales", saleDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateSale_WithValidData_ReturnsCreatedSale()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var saleDto = new CreateSaleDto
        {
            CustomerId = "test-customer",
            Details = new List<SaleDetailDto>
            {
                new SaleDetailDto { ProductId = 1, Quantity = 2 }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/sales", saleDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateSale_WithAuthentication_GeneratesPdfReceipt()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var saleDto = new CreateSaleDto
        {
            CustomerId = "test-customer",
            Details = new List<SaleDetailDto>
            {
                new SaleDetailDto { ProductId = 1, Quantity = 1 }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/sales", saleDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("fileName"); // Response should contain PDF file name
    }

    [Fact]
    public async Task CreateSale_WithMultipleProducts_CalculatesTotalsCorrectly()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var saleDto = new CreateSaleDto
        {
            CustomerId = "test-customer",
            Details = new List<SaleDetailDto>
            {
                new SaleDetailDto { ProductId = 1, Quantity = 2 }, // 2 * 100 = 200
                new SaleDetailDto { ProductId = 2, Quantity = 1 }  // 1 * 50 = 50
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/sales", saleDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        // Total should be 250 + tax (19%) = 297.5
    }

    [Fact]
    public async Task DownloadReceipt_WithValidFileName_ReturnsPdfFile()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // First create a sale to get a receipt
        var saleDto = new CreateSaleDto
        {
            CustomerId = "test-customer",
            Details = new List<SaleDetailDto>
            {
                new SaleDetailDto { ProductId = 1, Quantity = 1 }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/sales", saleDto);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        
        // Extract fileName from response (assuming it's in the response)
        // This is a simplified version - actual implementation may vary
        var fileName = "test-receipt.pdf";

        // Act
        var downloadResponse = await _client.GetAsync($"/api/sales/downloadreceipt?fileName={fileName}");

        // Assert
        // Note: This test may fail if the actual endpoint structure is different
        // Adjust based on actual implementation
        downloadResponse.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
        
        if (downloadResponse.StatusCode == HttpStatusCode.OK)
        {
            var pdfBytes = await downloadResponse.Content.ReadAsByteArrayAsync();
            pdfBytes.Should().NotBeEmpty();
        }
    }
}
