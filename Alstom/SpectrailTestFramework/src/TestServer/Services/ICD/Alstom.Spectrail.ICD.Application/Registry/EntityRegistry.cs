#region ©COPYRIGHT

// /*******************************************************************************
//  *   © COPYRIGHT ALSTOM SA 2025 - ALL RIGHTS RESERVED
//  *
//  *  This file is part of the Alstom Spectrail project.
//  *  Unauthorized copying, modification, distribution, or use of this file,
//  *  via any medium, is strictly prohibited.
//  *
//  *  Proprietary and confidential.
//  *  Created and maintained by Alstom for internal use in the Spectrail project.
//  ******************************************************************************/
// 
//  /*******************************************************************************
// AuthorName: SUBHASISH BISWAS
// Email: subhasish.biswas@alstomgroup.com
// FileName: EntityRegistry.cs
// ProjectName: Alstom.Spectrail.ICD.Application
// Created by SUBHASISH BISWAS On: 2025-03-12
// Updated by SUBHASISH BISWAS On: 2025-03-17
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.Server.Common.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using static MongoDB.Driver.Builders<Alstom.Spectrail.ICD.Application.Registry.EntityMapping>;

#endregion

namespace Alstom.Spectrail.ICD.Application.Registry;

/// <summary>
///     ✅ Manages entity mappings stored in MongoDB.
/// </summary>
public static class EntityRegistry
{
    private static IMongoCollection<EntityMapping>? _collection;

    /// <summary>
    ///     ✅ Initializes MongoDB connection.
    /// </summary>
    static EntityRegistry()
    {
        RegisterEntitiesFromAssembly();
    }

    public static void Initialize(IMongoCollection<EntityMapping>? collection)
    {
        _collection = collection;
    }

    /// <summary>
    ///     ✅ Gets the registered entity type using file name and sheet name.
    /// </summary>
    /// <summary>
    ///     ✅ Retrieves the registered entity type using the entity type name.
    /// </summary>
    public static Type? GetEntityType(string entityTypeName)
    {
        if (string.IsNullOrEmpty(entityTypeName))
        {
            Console.WriteLine("⚠️ Entity type name cannot be null or empty.");
            return null;
        }

        Console.WriteLine($"🔍 Searching Entity Type: {entityTypeName}");

        var fullyQualifiedName = GetFullyQualifiedEntityName(entityTypeName);

        Console.WriteLine($"📌 Found mapping: {fullyQualifiedName}");

        // ✅ Resolve the type from the current application domain
        var entityType = Type.GetType(fullyQualifiedName) ??
                         AppDomain.CurrentDomain.GetAssemblies()
                             .SelectMany(a => a.GetTypes())
                             .FirstOrDefault(t => t.FullName == fullyQualifiedName);

        if (entityType == null)
        {
            Console.WriteLine($"❌ Failed to resolve entity type: {fullyQualifiedName}");
            return null;
        }

        Console.WriteLine($"✅ Successfully resolved: {entityType.FullName}");
        return entityType;
    }

    /// <summary>
    ///     ✅ Registers a single entity dynamically in MongoDB.
    /// </summary>
    public static void RegisterEntity(string filePath, string sheetName, Type entityType)
    {
        var fileName = Path.GetFileName(filePath).Trim().ToLower();
        var normalizedSheetName = sheetName.Trim().Replace(" ", "").ToLower();
        var normalizedEntityName = entityType.FullName;

        var mapping = new EntityMapping
        {
            FileName = fileName,
            SheetName = normalizedSheetName,
            EntityName = normalizedEntityName ?? throw new InvalidOperationException()
        };

        var filter = Filter.And(
            Filter.Eq(x => x.FileName, mapping.FileName),
            Filter.Eq(x => x.SheetName, mapping.SheetName)
        );

        var update = Update
            .Set(x => x.EntityName, mapping.EntityName);

        var options = new UpdateOptions { IsUpsert = true }; // ✅ Upsert instead of insert

        _collection?.UpdateOne(filter, update, options);

        Console.WriteLine($"✅ Registered Entity: {entityType.FullName} for '{fileName}:{normalizedSheetName}'");
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
                .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(EntityBase).IsAssignableFrom(t))
                .ToList();

            foreach (var entityName in entityTypes.Select(type => type.Name.Replace("Entity", "").Trim().ToLower()))
                Console.WriteLine($"📌 Detected entity: {entityName}");
        }
    }


    /// <summary>
    ///     ✅ Returns all registered entity mappings.
    /// </summary>
    public static List<EntityMapping> GetAllMappings()
    {
        return _collection.Find(_ => true).ToList();
    }

    /// <summary>
    ///     ✅ Retrieves the fully qualified entity name from MongoDB.
    /// </summary>
    /// <summary>
    ///     ✅ Retrieves the fully qualified entity name from MongoDB based on the short entity name.
    /// </summary>
    public static string? GetFullyQualifiedEntityName(string shortEntityName)
    {
        if (_collection == null)
        {
            Console.WriteLine("❌ EntityRegistry collection is not initialized.");
            return null;
        }

        // ✅ Find entity where the fully qualified name ends with short entity name (case-insensitive)
        var filter = Filter.Regex(x => x.EntityName, new BsonRegularExpression($@"\.{shortEntityName}$", "i"));
        var result = _collection.Find(filter).FirstOrDefault();

        if (result == null)
        {
            Console.WriteLine($"⚠️ No match found for '{shortEntityName}' in EntityRegistry.");
            return null;
        }

        Console.WriteLine($"📌 Found fully qualified name: {result.EntityName}");
        return result.EntityName; // ✅ Example: "Alstom.Spectrail.ICD.Domain.Entities.ICD.BCHEntity"
    }
}

/// <summary>
///     ✅ Represents an entity mapping stored in MongoDB.
/// </summary>
public class EntityMapping
{
    [BsonId] // ✅ Marks _id as MongoDB's primary key (auto-generated)
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString(); // ✅ No fixed _id!

    public string FileName { get; init; } = string.Empty;
    public string SheetName { get; init; } = string.Empty;
    public string EntityName { get; init; } = string.Empty;
}