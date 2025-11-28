namespace Firmness.Domain.Entities;

public class Customer : ApplicationUser
{
    public ICollection<Sale> Sales { get; set; }
}