namespace Firmness.Domain.Entities;

/// <summary>
/// Represents a customer in the system, extending the application user.
/// </summary>
public class Customer : ApplicationUser
{
    /// <summary>
    /// Gets or sets the collection of sales associated with the customer.
    /// </summary>
    public ICollection<Sale> Sales { get; set; }
}