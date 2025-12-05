namespace Firmness.Application.MappingTemplates;

public static class CustomerColumnTemplate
{
    public static readonly Dictionary<string, string> Map = new()
    {
        { "username", "Username" },
        { "fullname", "FullName" },
        { "address", "Address" },
        { "phone", "Phone" },
        { "email", "Email" },
        { "created_at", "CreatedAt" }
    };
}
