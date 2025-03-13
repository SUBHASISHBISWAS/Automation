#region

using Alstom.Spectrail.Server.Common.Entities;

#endregion

namespace Alstom.Spectrail.ICD.Application.Registry;

public static class EntityRegistry
{
    // ReSharper disable once InconsistentNaming
    private static readonly Dictionary<string, Type> _entityMappings = new();

    /// <summary>
    ///     ✅ Auto-registers all entities dynamically.
    /// </summary>
    static EntityRegistry()
    {
        RegisterEntitiesFromAssembly();
    }

    /// <summary>
    ///     ✅ Gets the registered entity type by sheet name.
    /// </summary>
    public static Type? GetEntityType(string sheetName)
    {
        return _entityMappings.GetValueOrDefault(sheetName);
    }

    /// <summary>
    ///     ✅ Registers a single entity dynamically.
    /// </summary>
    public static void RegisterEntity(string sheetName, Type entityType)
    {
        if (typeof(EntityBase).IsAssignableFrom(entityType) && !_entityMappings.ContainsKey(sheetName))
            _entityMappings[sheetName] = entityType;
    }

    /// <summary>
    ///     ✅ Scans and registers all entities that inherit from EntityBase dynamically.
    /// </summary>
    private static void RegisterEntitiesFromAssembly()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(asm => asm.FullName!.StartsWith("Alstom.Spectrail"))
            .ToList();

        foreach (var asm in assemblies)
        {
            Console.WriteLine($"🔍 Scanning Assembly: {asm.FullName}");

            var entityTypes = asm.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(EntityBase).IsAssignableFrom(t))
                .ToList();

            foreach (var type in entityTypes)
            {
                var entityName = type.Name.Replace("Entity", "");
                if (!_entityMappings.TryAdd(entityName, type)) continue;
                Console.WriteLine($"✅ Registered Entity: {type.Name} as '{entityName}'");
            }
        }
    }

    /// <summary>
    ///     ✅ Returns all registered entity mappings.
    /// </summary>
    public static IReadOnlyDictionary<string, Type> GetAllMappings()
    {
        return _entityMappings;
    }
}