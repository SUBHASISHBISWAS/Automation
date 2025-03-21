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
// Created by SUBHASISH BISWAS On: 2025-03-21
// Updated by SUBHASISH BISWAS On: 2025-03-21
//  ******************************************************************************/

#endregion

#region

using System.Reflection;
using System.Reflection.Emit;
using Alstom.Spectrail.ICD.Application.Contracts;
using Alstom.Spectrail.ICD.Application.Models;
using Alstom.Spectrail.ICD.Application.Utility;
using Alstom.Spectrail.ICD.Domain.Entities.ICD;
using Alstom.Spectrail.Server.Common.Configuration;
using Alstom.Spectrail.Server.Common.Entities;
using ClosedXML.Excel;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using static MongoDB.Driver.Builders<Alstom.Spectrail.ICD.Application.Models.EntityMapping>;

#endregion

namespace Alstom.Spectrail.ICD.Application.Registry;

public class EntityRegistry
{
    private static IMongoCollection<EntityMapping>? _collection;
    private static IServerConfigHelper? _configHelper;
    private static IServiceCollection? _services;
    private static readonly Dictionary<string, Type> _entityTypeCache = new();
    private static readonly Dictionary<string, Type> _dynamicTypesCache = new();

    public EntityRegistry(IICDDbContext dbContext, IServerConfigHelper configHelper, IServiceCollection services)
    {
        _collection = dbContext.ICDEntityMapping;
        _configHelper = configHelper;
        _services = services;
    }

    public static Type? GetEntityType(string entityTypeName)
    {
        if (string.IsNullOrWhiteSpace(entityTypeName))
        {
            Console.WriteLine("⚠️ Entity type name cannot be null or empty.");
            return null;
        }

        if (_entityTypeCache.TryGetValue(entityTypeName.ToLower(), out var cachedType))
            return cachedType;

        var fullyQualifiedName = GetFullyQualifiedEntityName(entityTypeName);
        if (string.IsNullOrWhiteSpace(fullyQualifiedName))
            return null;

        var entityType = Type.GetType(fullyQualifiedName) ??
                         AppDomain.CurrentDomain.GetAssemblies()
                             .SelectMany(a => a.GetTypes())
                             .FirstOrDefault(t => t.FullName == fullyQualifiedName);

        if (entityType == null)
        {
            Console.WriteLine($"❌ Could not resolve: {fullyQualifiedName}");
            return null;
        }

        CacheEntityType(entityTypeName, entityType);
        return entityType;
    }

    public void RegisterEntity()
    {
        if (_entityTypeCache.Count() > 0) return;
        var icdFiles = _configHelper!.GetICDFiles();
        foreach (var filePath in icdFiles)
            try
            {
                var fileName = Path.GetFileName(filePath).Trim().ToLower();
                using var workbook = new XLWorkbook(filePath);
                var selectedSheets = ExtractEquipmentNames(filePath, _configHelper);

                foreach (var worksheet in workbook.Worksheets)
                {
                    var sheetName = worksheet.Name.Trim().Replace(" ", "").ToLower();

                    if (selectedSheets.Any() && !selectedSheets.Contains(sheetName, StringComparer.OrdinalIgnoreCase))
                    {
                        Console.WriteLine($"⚠️ Skipping sheet: {sheetName} (not in config)");
                        continue;
                    }

                    var entityType = GetEntityType(sheetName) ?? GenerateDynamicEntity(sheetName);
                    if (entityType == null) continue;

                    CacheEntityType(sheetName, entityType);

                    var mapping = new EntityMapping
                    {
                        FileName = fileName,
                        SheetName = sheetName,
                        EntityName = entityType.FullName!
                    };

                    var filter = Filter.And(
                        Filter.Eq(x => x.FileName, mapping.FileName),
                        Filter.Eq(x => x.SheetName, mapping.SheetName));

                    var existing = _collection?.Find(filter).FirstOrDefault();
                    if (existing != null && existing.EntityName == mapping.EntityName)
                    {
                        Console.WriteLine($"✅ Already registered: {mapping.EntityName}");
                        continue;
                    }

                    var update = Update.Set(x => x.EntityName, mapping.EntityName);
                    _collection?.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });

                    Console.WriteLine($"✅ Registered: {mapping.EntityName}");
                }

                var allDynamicTypes = _dynamicTypesCache.Values.ToList();
                DynamicRepositoryRegistrar.RegisterRepositoryHandlers(_services, allDynamicTypes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error processing file: {filePath}, Msg: {ex.Message}");
            }
    }

    public static List<string> ExtractEquipmentNames(string filePath, IServerConfigHelper configHelper)
    {
        using var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet("network_config");

        if (worksheet == null)
            throw new InvalidOperationException("⚠️ Sheet 'network_config' not found.");

        var headerRow = worksheet.Row(1).Cells().Select(c => c.GetString().Trim()).ToList();
        var colIndex = headerRow.IndexOf("Equipment Sheet");

        if (colIndex == -1)
            throw new InvalidOperationException("⚠️ Column 'Equipment Sheet' not found.");

        var allEquipments = worksheet.Column(colIndex + 1)
            .CellsUsed().Skip(1).Select(c => c.GetString().Trim())
            .Distinct().ToList();

        var fileName = Path.GetFileName(filePath).ToLower();
        var filters = configHelper.GetSection<Dictionary<string, List<string>>>("Settings:DynamicEntityFilters");

        filters.TryGetValue(fileName, out var perFile);
        filters.TryGetValue("default", out var fallback);

        var allowedPrefixes = perFile ?? fallback ?? new List<string>();

        return allowedPrefixes.Count == 0
            ? allEquipments
            : allEquipments.Where(name =>
                allowedPrefixes.Any(prefix =>
                    name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))).ToList();
    }

    public static List<EntityMapping> GetAllMappings()
    {
        return _collection?.Find(_ => true).ToList() ?? [];
    }

    private static string? GetFullyQualifiedEntityName(string shortEntityName)
    {
        if (_collection == null) return null;

        var filter = Filter.Regex(x => x.EntityName, new BsonRegularExpression($@"\.{shortEntityName}$", "i"));
        return _collection.Find(filter).FirstOrDefault()?.EntityName;
    }

    public static void CacheEntityType(string entityTypeName, Type entityType)
    {
        if (!_entityTypeCache.TryAdd(entityTypeName.ToLower(), entityType)) return;
        Console.WriteLine($"🧠 Cached: {entityType.FullName}");
    }

    public static Type GenerateDynamicEntity(string entityName)
    {
        // Normalize to PascalCase and append 'Entity'
        var pascalName = char.ToUpper(entityName[0]) + entityName[1..].ToLower();
        var fullTypeName = $"Alstom.Spectrail.ICD.Domain.Entities.ICD.{pascalName}Entity";

        if (_dynamicTypesCache.TryGetValue(fullTypeName, out var existingType))
            return existingType;

        var assemblyName = new AssemblyName("DynamicEntities");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

        var typeBuilder = moduleBuilder.DefineType(
            fullTypeName,
            TypeAttributes.Public | TypeAttributes.Class,
            typeof(EntityBase)
        );

        // ✅ Reuse property definition from a template like DCUEntity
        // 🔍 Get all public instance properties from the base class
        var baseProperties = typeof(EntityBase)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase); // Ignore case for safety

// 🔍 Get all properties from the template class (e.g., DCUEntity)
        var properties = typeof(DCUEntity)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => !baseProperties.Contains(p.Name)) // 🧠 Skip inherited ones
            .ToList();
        foreach (var prop in properties)
        {
            var fieldBuilder = typeBuilder.DefineField($"_{char.ToLower(prop.Name[0]) + prop.Name[1..]}",
                prop.PropertyType, FieldAttributes.Private);

            var getter = typeBuilder.DefineMethod($"get_{prop.Name}",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                prop.PropertyType, Type.EmptyTypes);
            var getterIl = getter.GetILGenerator();
            getterIl.Emit(OpCodes.Ldarg_0);
            getterIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getterIl.Emit(OpCodes.Ret);

            var setter = typeBuilder.DefineMethod($"set_{prop.Name}",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                null, new[] { prop.PropertyType });
            var setterIl = setter.GetILGenerator();
            setterIl.Emit(OpCodes.Ldarg_0);
            setterIl.Emit(OpCodes.Ldarg_1);
            setterIl.Emit(OpCodes.Stfld, fieldBuilder);
            setterIl.Emit(OpCodes.Ret);

            var propertyBuilder =
                typeBuilder.DefineProperty(prop.Name, PropertyAttributes.HasDefault, prop.PropertyType, null);
            propertyBuilder.SetGetMethod(getter);
            propertyBuilder.SetSetMethod(setter);
        }

        var dynamicType = typeBuilder.CreateType();
        _dynamicTypesCache[fullTypeName] = dynamicType;

        Console.WriteLine($"🛠 Generated dynamic entity: {fullTypeName}");

        return dynamicType;
    }
}