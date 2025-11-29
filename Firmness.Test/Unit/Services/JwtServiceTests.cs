using Firmness.Infrastructure.Services.Identity;
using Firmness.Domain.Entities;
using Microsoft.Extensions.Configuration;
using FluentAssertions;
using Xunit;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Firmness.Test.Unit.Services;

public class JwtServiceTests
{
    private readonly JwtService _jwtService;
    private readonly IConfiguration _configuration;

    public JwtServiceTests()
    {
        // Setup configuration for JWT
        var inMemorySettings = new Dictionary<string, string>
        {
            {"Jwt:Key", "ThisIsAVerySecureKeyForTestingPurposesOnly123456"},
            {"Jwt:Issuer", "FirmnessTestIssuer"},
            {"Jwt:Audience", "FirmnessTestAudience"},
            {"Jwt:ExpireMinutes", "60"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _jwtService = new JwtService(_configuration);
    }

    [Fact]
    public void GenerateToken_WithValidUser_ReturnsValidJwt()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "user-123",
            UserName = "testuser@example.com",
            Email = "testuser@example.com",
            FullName = "Test User"
        };
        var roles = new List<string> { "Customer" };

        // Act
        var token = _jwtService.GenerateToken(user, roles);

        // Assert
        token.Should().NotBeNullOrEmpty();
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        jwtToken.Should().NotBeNull();
        jwtToken.Issuer.Should().Be("FirmnessTestIssuer");
        jwtToken.Audiences.Should().Contain("FirmnessTestAudience");
    }

    [Fact]
    public void GenerateToken_WithUser_IncludesUserIdClaim()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "user-123",
            UserName = "testuser@example.com",
            Email = "testuser@example.com",
            FullName = "Test User"
        };
        var roles = new List<string> { "Customer" };

        // Act
        var token = _jwtService.GenerateToken(user, roles);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        userIdClaim.Should().NotBeNull();
        userIdClaim!.Value.Should().Be("user-123");
    }

    [Fact]
    public void GenerateToken_WithRoles_IncludesRoleClaims()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "user-123",
            UserName = "testuser@example.com",
            Email = "testuser@example.com",
            FullName = "Test User"
        };
        var roles = new List<string> { "Admin", "Customer" };

        // Act
        var token = _jwtService.GenerateToken(user, roles);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
        roleClaims.Should().HaveCount(2);
        roleClaims.Select(c => c.Value).Should().Contain(new[] { "Admin", "Customer" });
    }

    [Fact]
    public void GenerateToken_WithUser_IncludesEmailClaim()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "user-123",
            UserName = "testuser@example.com",
            Email = "testuser@example.com",
            FullName = "Test User"
        };
        var roles = new List<string> { "Customer" };

        // Act
        var token = _jwtService.GenerateToken(user, roles);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
        emailClaim.Should().NotBeNull();
        emailClaim!.Value.Should().Be("testuser@example.com");
    }

    [Fact]
    public void GenerateToken_ChecksExpiration_IsSetCorrectly()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "user-123",
            UserName = "testuser@example.com",
            Email = "testuser@example.com",
            FullName = "Test User"
        };
        var roles = new List<string> { "Customer" };

        // Act
        var token = _jwtService.GenerateToken(user, roles);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        var expirationTime = jwtToken.ValidTo;
        var expectedExpiration = DateTime.UtcNow.AddMinutes(60);
        
        expirationTime.Should().BeCloseTo(expectedExpiration, TimeSpan.FromMinutes(1));
    }
}
