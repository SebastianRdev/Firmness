namespace Firmness.Application.DTOs.Customers;

/// <summary>
/// Data transfer object for customer details.
/// </summary>
public class CustomerDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the customer.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the full name.
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the address.
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// Gets or sets the phone number.
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the list of roles assigned to the customer.
    /// </summary>
    public List<string> Roles { get; set; }
}
