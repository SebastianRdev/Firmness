using System;
using System.Collections.Generic;

namespace Firmness.Application.MappingTemplates;

public static class ColumnTemplateFactory
{
    public static Dictionary<string, string> GetTemplate(string entityType)
    {
        entityType = entityType.ToLower();

        Dictionary<string, string> result = entityType switch
        {
            "customer" => CustomerColumnTemplate.Map,
            "product"  => ProductColumnTemplate.Map,
            _ => throw new ArgumentException($"No existe plantilla para {entityType}")
        };

        return result;
    }
}
