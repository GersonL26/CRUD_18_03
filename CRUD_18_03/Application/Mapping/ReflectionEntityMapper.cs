using System.Reflection;

namespace CRUD_18_03.Application.Mapping;

public class ReflectionEntityMapper : IEntityMapper
{
    public object MapToNew(object source, Type destinationType)
    {
        var destination = Activator.CreateInstance(destinationType)
            ?? throw new InvalidOperationException(
                $"No se pudo crear una instancia de '{destinationType.Name}'.");

        var sourceProperties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var destinationProperties = destinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var sourceProperty in sourceProperties)
        {
            var destinationProperty = FindMatchingProperty(destinationProperties, sourceProperty.Name);

            if (destinationProperty is null || !destinationProperty.CanWrite)
                continue;

            var value = sourceProperty.GetValue(source);
            destinationProperty.SetValue(destination, value);
        }

        return destination;
    }

    public void MapToExisting(object source, object destination)
    {
        var sourceProperties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var destinationProperties = destination.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var sourceProperty in sourceProperties)
        {
            var value = sourceProperty.GetValue(source);

            if (value is null)
                continue;

            var destinationProperty = FindMatchingProperty(destinationProperties, sourceProperty.Name);

            if (destinationProperty is null || !destinationProperty.CanWrite)
                continue;

            destinationProperty.SetValue(destination, value);
        }
    }

    private static PropertyInfo? FindMatchingProperty(PropertyInfo[] properties, string name)
    {
        return Array.Find(properties,
            p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
    }
}
