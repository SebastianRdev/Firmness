namespace Firmness.Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // One-to-many relationship (one category has many products)
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
