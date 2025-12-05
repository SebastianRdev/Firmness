namespace Firmness.Application.DTOs.Auth;

/// <summary>
/// Data transfer object for authentication response.
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// Gets or sets the user's unique identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the JWT token.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's full name.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of roles assigned to the user.
    /// </summary>
    public IList<string> Roles { get; set; } = new List<string>();
}
