#region

using System.Reflection;
using Alstom.Spectrail.ICD.Domain.Common;
using Alstom.Spectrail.Server.Common.Entities;

#endregion

namespace Alstom.Spectrail.ICD.Application.Registry;

public static class EntityRegistry
{
    private static readonly Dictionary<string, Type> _entityMappings = new();

    /// <summary>
    ///     ✅ Auto-registers all entities dynamically.
    /// </summary>
    static EntityRegistry()
    {
        RegisterEntitiesFromAssembly(Assembly.GetAssembly(typeof(EntityBase)));
    }

    public static Type? GetEntityType(string sheetName)
    {
        return _entityMappings.TryGetValue(sheetName, out var entityType) ? entityType : null;
    }

    public static void RegisterEntity(string sheetName, Type entityType)
    {
        if (typeof(EntityBase).IsAssignableFrom(entityType) && !_entityMappings.ContainsKey(sheetName))
            _entityMappings[sheetName] = entityType;
    }

    public static void RegisterEntitiesFromAssembly(Assembly? assembly)
    {
        var entityTypes = assembly.GetTypes()
            .Where(t => typeof(EntityBase).IsAssignableFrom(t) && !t.IsAbstract)
            .ToList();

        foreach (var type in entityTypes)
        {
            var entityName = type.Name.Replace("Entity", "");
            if (!_entityMappings.ContainsKey(entityName)) _entityMappings[entityName] = type;
        }
    }

    public static IReadOnlyDictionary<string, Type> GetAllMappings()
    {
        return _entityMappings;
    }
}