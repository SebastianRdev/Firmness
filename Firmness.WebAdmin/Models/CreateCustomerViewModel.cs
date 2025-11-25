namespace Firmness.WebAdmin.Models;

using System.ComponentModel.DataAnnotations;

public class CreateCustomerViewModel
{
    [Required(ErrorMessage = "Username is required.")]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters.")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Full name is required.")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email format is invalid.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    public string Address { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    [Phone(ErrorMessage = "Phone number format is invalid.")]
    public string PhoneNumber { get; set; }

    // === PASSWORD RULES (match Identity config) ===
    // RequiredLength = 6
    // RequireUppercase = false
    // RequireLowercase = false
    // RequireDigit = false
    // RequireNonAlphanumeric = false
    // RequiredUniqueChars = 0
    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must have at least 6 characters.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "You must confirm your password.")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }
}