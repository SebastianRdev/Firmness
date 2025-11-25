namespace Firmness.WebAdmin.Models;

using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

public class EditCustomerViewModel
{
    [Required]
    public Guid Id { get; set; }

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

    // ==== ROLE ====
    [Required(ErrorMessage = "A role must be selected.")]
    public string SelectedRole { get; set; }

    public List<SelectListItem> Roles { get; set; }
    
    // ==== PASSWORD CHANGE (optional) ====
    // Apply Identity rules ONLY if user enters a new password

    [MinLength(6, ErrorMessage = "Password must have at least 6 characters.")]
    public string NewPassword { get; set; }

    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    public string ConfirmNewPassword { get; set; }
}
