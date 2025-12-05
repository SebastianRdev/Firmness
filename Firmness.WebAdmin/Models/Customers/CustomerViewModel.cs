namespace Firmness.WebAdmin.Models.Customers;

using Microsoft.AspNetCore.Mvc.Rendering;

/// <summary>
/// ViewModel for displaying customer details.
/// </summary>
public class CustomerViewModel
{
    /// <summary>
    /// Gets or sets the customer ID.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the full name of the customer.
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string Email { get; set; } 

    /// <summary>
    /// Gets or sets the physical address.
    /// </summary>
    public string Address { get; set; } 

    /// <summary>
    /// Gets or sets the phone number.
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the password (usually not displayed).
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets the password confirmation (usually not displayed).
    /// </summary>
    public string ConfirmPassword { get; set; }

    /// <summary>
    /// Gets or sets the list of roles assigned to the customer.
    /// </summary>
    public List<SelectListItem> Roles { get; set; }
}
