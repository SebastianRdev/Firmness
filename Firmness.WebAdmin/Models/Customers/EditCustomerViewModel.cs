namespace Firmness.WebAdmin.Models.Customers;

/// <summary>
/// ViewModel for editing an existing customer.
/// </summary>
public class EditCustomerViewModel
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

    // Password fields (Optional)
    /// <summary>
    /// Gets or sets the new password (optional).
    /// </summary>
    public string? NewPassword { get; set; }

    /// <summary>
    /// Gets or sets the confirmation of the new password (optional).
    /// </summary>
    public string? ConfirmNewPassword { get; set; }
}