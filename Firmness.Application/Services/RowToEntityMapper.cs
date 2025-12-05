using System;
using System.Collections.Generic;
using System.Reflection;

namespace Firmness.Application.Services;

public static class RowToEntityMapper
{
    public static T MapRow<T>(Dictionary<string, string> row, Dictionary<string, string> template)
        where T : new()
    {
        var entity = new T();
        var type = typeof(T);

        foreach (var kv in row)
        {
            var incomingKey = kv.Key.ToLower();

            if (!template.ContainsKey(incomingKey))
                continue;

            var propertyName = template[incomingKey];
            var prop = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

            if (prop == null) continue;

            try
            {
                object? converted = Convert.ChangeType(kv.Value, prop.PropertyType);
                prop.SetValue(entity, converted);
            }
            catch
            {
                // En producción podrías registrar errores por fila.
                continue;
            }
        }

        return entity;
    }
}
