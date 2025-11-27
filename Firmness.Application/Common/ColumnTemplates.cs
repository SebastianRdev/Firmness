namespace Firmness.Application.Common;

public static class ColumnTemplates
{
    public static readonly Dictionary<string, List<string>> Templates = new Dictionary<string, List<string>>
    {
        { 
            "Customer", new List<string> { "username", "address", "phone", "email" } 
        },
        { 
            "Product", new List<string> { "productname", "price", "category", "stock" } 
        }
    };
}