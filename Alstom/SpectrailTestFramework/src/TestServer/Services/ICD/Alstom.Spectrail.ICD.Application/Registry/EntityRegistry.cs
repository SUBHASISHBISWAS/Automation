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
// Created by SUBHASISH BISWAS On: 2025-03-20
// Updated by SUBHASISH BISWAS On: 2025-03-20
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.Application.Contracts;
using Alstom.Spectrail.ICD.Application.Models;
using Alstom.Spectrail.Server.Common.Configuration;
using Alstom.Spectrail.Server.Common.Entities;
using ClosedXML.Excel;
using MongoDB.Bson;
using MongoDB.Driver;
using static MongoDB.Driver.Builders<Alstom.Spectrail.ICD.Application.Models.EntityMapping>;

#endregion

namespace Alstom.Spectrail.ICD.Application.Registry;

/// <summary>
///     ✅ Manages entity mappings stored in MongoDB.
/// </summary>
public class EntityRegistry
{
    private static IMongoCollection<EntityMapping>? _collection;
    private static IServerConfigHelper? _configHelper;
    private static readonly Dictionary<string, Type> _entityTypeCache = new();

    /// <summary>
    ///     ✅ Initializes MongoDB connection.
    /// </summary>
    public EntityRegistry(IICDDbContext dbContext, IServerConfigHelper configHelper)
    {
        _collection = dbContext.ICDEntityMapping;
        _configHelper = configHelper;
        RegisterEntitiesFromAssembly();
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
    ///     ✅ Now updates only if changes are found.
    /// </summary>
    public void RegisterEntity()
    {
        var icdFiles = _configHelper!.GetICDFiles();
        foreach (var filePath in icdFiles)
            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"❌ File not found: {filePath}");

                using var workbook = new XLWorkbook(filePath);
                var fileName = Path.GetFileName(filePath).Trim().ToLower();
                foreach (var worksheet in workbook.Worksheets)
                {
                    var normalizedSheetName = worksheet.Name.Trim().Replace(" ", "").ToLower();
                    Console.WriteLine($"📌 Processing sheet: {normalizedSheetName}");

                    // ✅ Step 1: Check Cache First
                    if (!_entityTypeCache.TryGetValue(normalizedSheetName, out var cachedType))
                    {
                        Console.WriteLine($"⚠️ No registered entity for sheet: {normalizedSheetName}. Skipping...");
                        continue;
                    }

                    var normalizedEntityName = cachedType.FullName;
                    var mapping = new EntityMapping
                    {
                        FileName = fileName,
                        SheetName = normalizedSheetName,
                        EntityName = normalizedEntityName
                    };

                    var filter = Filter.And(
                        Filter.Eq(x => x.FileName, mapping.FileName),
                        Filter.Eq(x => x.SheetName, mapping.SheetName)
                    );

                    // ✅ Check if an update is actually needed
                    var existingEntity = _collection?.Find(filter).FirstOrDefault();
                    if (existingEntity != null && existingEntity.EntityName == mapping.EntityName)
                    {
                        Console.WriteLine(
                            $"✅ Already Registered: {cachedType.FullName} for '{fileName}:{normalizedSheetName}'");
                        continue;
                    }

                    // ✅ If change detected, update the entity mapping
                    var update = Update.Set(x => x.EntityName, mapping.EntityName);
                    var options = new UpdateOptions { IsUpsert = true };

                    _collection?.UpdateOne(filter, update, options);
                    Console.WriteLine(
                        $"✅ Updated Entity: {cachedType.FullName} for '{fileName}:{normalizedSheetName}'");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error processing {filePath}: {ex.Message}");
            }
    }

    /// <summary>
    ///     ✅ Caches resolved entity types in memory for faster lookups.
    /// </summary>
    private static void CacheEntityType(string entityTypeName, Type entityType)
    {
        if (!_entityTypeCache.TryAdd(entityTypeName, entityType)) return;
        Console.WriteLine($"🛠 Cached entity type: {entityType.FullName}");
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

            foreach (var type in entityTypes)
            {
                var shortName = type.Name.Replace("Entity", "").Trim().ToLower();
                Console.WriteLine($"📌 Detected entity: {shortName}");
                CacheEntityType(shortName, type); // ✅ Cache entity types
            }
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
    private static string? GetFullyQualifiedEntityName(string shortEntityName)
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