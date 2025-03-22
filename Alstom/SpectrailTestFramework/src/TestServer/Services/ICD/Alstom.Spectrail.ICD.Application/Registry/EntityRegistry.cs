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
// Updated by SUBHASISH BISWAS On: 2025-03-22
//  ******************************************************************************/

#endregion

#region

using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;
using Alstom.Spectrail.ICD.Application.Contracts;
using Alstom.Spectrail.ICD.Application.Models;
using Alstom.Spectrail.ICD.Application.Utility;
using Alstom.Spectrail.ICD.Domain.DTO.ICD;
using Alstom.Spectrail.ICD.Domain.Entities.ICD;
using Alstom.Spectrail.Server.Common.Configuration;
using Alstom.Spectrail.Server.Common.Entities;
using AutoMapper;
using ClosedXML.Excel;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using StackExchange.Redis;
using static MongoDB.Driver.Builders<Alstom.Spectrail.ICD.Application.Models.EntityMapping>;

#endregion

namespace Alstom.Spectrail.ICD.Application.Registry;

public class EntityRegistry
{
    private static IMongoCollection<EntityMapping> _collection;
    private static IServerConfigHelper _configHelper;
    private static IServiceCollection _services;
    private static IMapperConfigurationExpression _mapperConfig;
    private static IDatabase _redis;
    private static readonly Dictionary<string, Type> _localDynamicTypesCache = new();
    private static Dictionary<string, List<string>> _registeredEntityByFile = new();

    public EntityRegistry(IICDDbContext dbContext, IServerConfigHelper configHelper, IServiceCollection services,
        IMapperConfigurationExpression mapperConfig, IConnectionMultiplexer redis)
    {
        ArgumentNullException.ThrowIfNull(dbContext, nameof(IMongoCollection<EntityMapping>));
        ArgumentNullException.ThrowIfNull(configHelper, nameof(IServerConfigHelper));
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));
        ArgumentNullException.ThrowIfNull(redis, nameof(IDatabase));
        ArgumentNullException.ThrowIfNull(mapperConfig, nameof(IMapperConfigurationExpression));

        Debug.Assert(dbContext.ICDEntityMapping != null);
        _collection = dbContext.ICDEntityMapping;
        _configHelper = configHelper;
        _services = services;
        _mapperConfig = mapperConfig;
        _redis = redis.GetDatabase();
    }

    public static Type? GetEntityType(string entityTypeName)
    {
        if (string.IsNullOrWhiteSpace(entityTypeName))
        {
            Console.WriteLine("⚠️ Entity type name cannot be null or empty.");
            return null;
        }

        var pascalName = char.ToUpper(entityTypeName[0]) + entityTypeName[1..].ToLower();
        var fullTypeName = $"{SpectrailConstants.DynamicAssemblyName}.{pascalName}Entity";

        // 🔍 Step 1: Local cache
        if (_localDynamicTypesCache.TryGetValue(fullTypeName, out var cachedType))
            return cachedType;

        // 🌐 Step 2: Redis check
        var redisKey = $"{SpectrailConstants.RedisDynamicType}{fullTypeName}";
        if (_redis.KeyExists(redisKey))
        {
            var resolvedType = Type.GetType(fullTypeName)
                               ?? AppDomain.CurrentDomain
                                   .GetAssemblies()
                                   .SelectMany(SpectralUtility.SafeGetTypes)
                                   .FirstOrDefault(t => t.FullName == fullTypeName);

            if (resolvedType != null)
            {
                _localDynamicTypesCache[fullTypeName] = resolvedType;
                Console.WriteLine($"✅ Resolved from Redis: {fullTypeName}");
                return resolvedType;
            }

            Console.WriteLine($"⚠️ Redis entry exists but type not loaded: {fullTypeName}");
        }

        // ❌ Step 3: Not found
        Console.WriteLine($"❌ Could not resolve entity type: {fullTypeName}");
        return null;
    }

    private static IXLWorksheets GetAllWorksheets(string filePath, out List<string> allEquipments,
        out List<string> equipmentsToRegister)
    {
        //var fileName = Path.GetFileName(filePath).ToLower();
        using var workbook = new XLWorkbook(filePath);
        var networkSheet = workbook.Worksheet($"{SpectrailConstants.ICD_NetworkConfig}");
        if (networkSheet == null) throw new InvalidOperationException("⚠️ Sheet 'network_config' not found.");

        var headerRow = networkSheet.Row(1).Cells().Select(c => c.GetString().Trim()).ToList();
        var colIndex = headerRow.IndexOf("Equipment Sheet");
        if (colIndex == -1) throw new InvalidOperationException("⚠️ Column 'Equipment Sheet' not found.");

        allEquipments = networkSheet.Column(colIndex + 1)
            .CellsUsed().Skip(1).Select(c => c.GetString().Trim())
            .Distinct().ToList();

        var filters =
            _configHelper.GetSection<Dictionary<string, List<string>>>(
                $"{SpectrailConstants.Settings_DynamicEntityFilters}");
        filters.TryGetValue(filePath.GetFileName(), out var perFile);
        filters.TryGetValue("default", out var fallback);
        var allowedPrefixes = perFile ?? fallback ?? [];

        equipmentsToRegister = allowedPrefixes.Count == 0
            ? allEquipments
            : allEquipments.Where(name =>
                allowedPrefixes.Any(prefix =>
                    name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))).ToList();
        _registeredEntityByFile[filePath.GetFileNameWithoutExtension()] = equipmentsToRegister;
        return workbook.Worksheets;
    }

    private static (List<Type> StoredEntities, List<Type> RegisterEntity)
        ProcessExcelAndRegisterEntitiesAndExtractEquipmentNames(string filePath)
    {
        var storedEntity = new List<Type>();
        var registeredEntity = new List<Type>();
        //var fileName = Path.GetFileName(filePath).ToLower();
        var workSheets = GetAllWorksheets(filePath, out var allEquipments, out var equipmentsToRegister);
        try
        {
            var fileHashKey = $"{SpectrailConstants.RedisFileHashKey}{filePath.GetFileNameWithoutExtension()}";
            var storedHash = _redis?.StringGet(fileHashKey).ToString();
            var currentHash = filePath.ComputeFileHash();

            if (storedHash != currentHash)
            {
                Console.WriteLine($"🧊 File Changed: {filePath.GetFileName()}. Skipping processing.");
                return ([], []);
            }

            var equipmentKey = $"{SpectrailConstants.RedisEquipmentHashKey}{filePath.GetFileNameWithoutExtension()}";
            _redis?.KeyDelete(equipmentKey); // Clear previous

            foreach (var equipment in allEquipments)
                _redis?.HashSet(equipmentKey, equipment, equipmentsToRegister.Contains(equipment).ToString());

            foreach (var worksheet in workSheets)
            {
                var sheetName = worksheet.Name.Trim().Replace(" ", "").ToUpper();
                var isRegistered = equipmentsToRegister.Contains(sheetName, StringComparer.OrdinalIgnoreCase);
                var networkSheet = allEquipments.Contains(sheetName, StringComparer.OrdinalIgnoreCase);
                if (!networkSheet) continue;
                var entityType = GetEntityType(sheetName) ?? GenerateDynamicEntity(sheetName);
                _mapperConfig.CreateMap(entityType, typeof(CustomColumnDto)).ReverseMap();
                Debug.Assert(entityType.FullName != null);
                var mapping = new EntityMapping
                {
                    FileName = filePath.GetFileName(),
                    SheetName = sheetName,
                    EntityName = entityType.FullName,
                    IsRegistered = isRegistered
                };

                var filter = Filter.And(
                    Filter.Eq(x => x.FileName, mapping.FileName),
                    Filter.Eq(x => x.SheetName, mapping.SheetName));

                var update = Update
                    .Set(x => x.EntityName, mapping.EntityName)
                    .Set(x => x.IsRegistered, mapping.IsRegistered);

                _collection?.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });

                Console.WriteLine($"📄 Processed sheet: {sheetName}, Registered: {isRegistered}");

                if (isRegistered) registeredEntity.Add(entityType);

                storedEntity.Add(entityType);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error processing file: {filePath}, Msg: {ex.Message}");
        }

        return (storedEntity, registeredEntity);
    }

    public List<Type> RegisterEntity()
    {
        var registeredEntityTypes = new List<Type>();
        foreach (var filePath in _configHelper.GetICDFiles())
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"⚠️ File not found: {filePath}");
                    continue;
                }

                var entityTypes = ProcessExcelAndRegisterEntitiesAndExtractEquipmentNames(filePath);

                if (entityTypes.StoredEntities.Count == 0)
                {
                    Console.WriteLine($"⚠️ No entity types found in file: {filePath}");
                    continue;
                }

                if (entityTypes.RegisterEntity.Count == 0)
                {
                    Console.WriteLine($"⚠️ No entity types registered: {filePath}");
                    continue;
                }

                registeredEntityTypes.AddRange(entityTypes.RegisterEntity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error processing file: {filePath}, Msg: {ex.Message}");
            }

        _ = PersistRegisteredEntitiesAsync();
        return registeredEntityTypes.Distinct().ToList(); // ensure no duplicates
    }

    private static async Task PersistRegisteredEntitiesAsync()
    {
        var json = JsonSerializer.Serialize(_registeredEntityByFile);
        await _redis.StringSetAsync(SpectrailConstants.RedisEntityListKey, json);
    }

    private static async Task LoadRegisteredEntitiesAsync()
    {
        var json = await _redis.StringGetAsync(SpectrailConstants.RedisEntityListKey);
        if (!json.IsNullOrEmpty)
            _registeredEntityByFile = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json)
                                      ?? new Dictionary<string, List<string>>();
    }


    public static List<string>? ExtractEquipmentNames(string filePath)
    {
        var fileName = Path.GetFileName(filePath).ToLower();
        if (_registeredEntityByFile.Count == 0) _ = LoadRegisteredEntitiesAsync();
        _registeredEntityByFile.TryGetValue(fileName, out var registeredEntity);
        return registeredEntity is { Count: > 0 } ? registeredEntity : null;
    }

    public static List<EntityMapping> GetAllMappings()
    {
        return _collection.Find(_ => true).ToList() ?? [];
    }

    private static string? GetFullyQualifiedEntityName(string shortEntityName)
    {
        if (_collection == null) return null;

        var filter = Filter.Regex(x => x.EntityName,
            new BsonRegularExpression($@"\.{shortEntityName}$", "i"));
        return _collection.Find(filter).FirstOrDefault()?.EntityName;
    }


    private static Type GenerateDynamicEntity(string entityName)
    {
        var pascalName = char.ToUpper(entityName[0]) + entityName[1..].ToLower();
        var fullTypeName = $"{SpectrailConstants.DynamicAssemblyName}.{pascalName}Entity";

        // ⚡ In-memory check
        if (_localDynamicTypesCache.TryGetValue(fullTypeName, out var cachedType))
            return cachedType;

        // 🌐 Redis check
        var redisKey = $"{SpectrailConstants.RedisDynamicType}{fullTypeName}";
        if (_redis!.KeyExists(redisKey))
        {
            var type = Type.GetType(fullTypeName);
            if (type != null)
            {
                _localDynamicTypesCache[fullTypeName] = type;
                Console.WriteLine($"✅ Retrieved cached entity type from Redis: {fullTypeName}");
                return type;
            }
        }

        // 🏗 Build dynamically
        var assemblyName = new AssemblyName($"{SpectrailConstants.DynamicAssemblyName}.DynamicEntities");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

        var typeBuilder = moduleBuilder.DefineType(
            fullTypeName,
            TypeAttributes.Public | TypeAttributes.Class,
            typeof(EntityBase)
        );

        var baseProps = typeof(EntityBase)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var props = typeof(DCUEntity)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => !baseProps.Contains(p.Name))
            .ToList();

        foreach (var prop in props)
        {
            var fieldBuilder = typeBuilder.DefineField($"_{char.ToLower(prop.Name[0]) + prop.Name[1..]}",
                prop.PropertyType, FieldAttributes.Private);

            var getter = typeBuilder.DefineMethod($"get_{prop.Name}",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, prop.PropertyType,
                Type.EmptyTypes);
            var getterIl = getter.GetILGenerator();
            getterIl.Emit(OpCodes.Ldarg_0);
            getterIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getterIl.Emit(OpCodes.Ret);

            var setter = typeBuilder.DefineMethod($"set_{prop.Name}",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null,
                [prop.PropertyType]);
            var setterIl = setter.GetILGenerator();
            setterIl.Emit(OpCodes.Ldarg_0);
            setterIl.Emit(OpCodes.Ldarg_1);
            setterIl.Emit(OpCodes.Stfld, fieldBuilder);
            setterIl.Emit(OpCodes.Ret);

            var propBuilder =
                typeBuilder.DefineProperty(prop.Name, PropertyAttributes.HasDefault, prop.PropertyType, null);
            propBuilder.SetGetMethod(getter);
            propBuilder.SetSetMethod(setter);
        }

        var dynamicType = typeBuilder.CreateType();

        // 💾 Update in-memory cache
        _localDynamicTypesCache[fullTypeName] = dynamicType;

        // 💾 Store in Redis (for next runs)
        _redis.StringSet(redisKey, "true"); // Could store timestamp or metadata if needed

        Console.WriteLine($"🛠 Generated and cached dynamic entity: {fullTypeName}");

        return dynamicType;
    }
}