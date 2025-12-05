using System;
using System.Collections.Generic;
using System.Reflection;

namespace Firmness.Application.Services;

/// <summary>
/// Utility class for mapping dictionary rows to entity properties based on a template.
/// </summary>
public static class RowToEntityMapper
{
    /// <summary>
    /// Maps a row of data (dictionary) to an entity of type T using a template mapping.
    /// </summary>
    /// <typeparam name="T">The target entity type.</typeparam>
    /// <param name="row">The dictionary containing the row data.</param>
    /// <param name="template">The dictionary mapping column names to property names.</param>
    /// <returns>A new instance of T with populated properties.</returns>
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
