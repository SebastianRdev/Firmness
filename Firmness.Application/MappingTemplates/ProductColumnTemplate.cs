namespace Firmness.Application.MappingTemplates;

public static class ProductColumnTemplate
{
    public static readonly Dictionary<string, string> Map = new()
    {
        { "name", "Name" },
        { "category", "Category" },
        { "price", "Price" },
        { "stock", "Stock" },
        { "description", "Description" },
        { "created_at", "CreatedAt" },
        { "updated_at", "UpdatedAt" }
    };
}
