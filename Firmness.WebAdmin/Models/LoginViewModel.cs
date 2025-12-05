namespace Firmness.WebAdmin.Models;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// ViewModel for user login.
/// </summary>
public class LoginViewModel
{
    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's password.
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether to remember the user's login session.
    /// </summary>
    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
}
