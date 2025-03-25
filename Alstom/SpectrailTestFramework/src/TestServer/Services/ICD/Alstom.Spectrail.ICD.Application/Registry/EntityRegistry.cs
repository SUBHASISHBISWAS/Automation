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
// Created by SUBHASISH BISWAS On: 2025-03-25
// Updated by SUBHASISH BISWAS On: 2025-03-26
//  ******************************************************************************/

#endregion

#region

using System.Collections.Concurrent;
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
    private static IMapperConfigurationExpression _mapperConfig;
    private static IDatabase _redis;


    private static readonly ConcurrentDictionary<string, List<IXLWorksheet>> icdWorksheetMap =
        new(StringComparer.OrdinalIgnoreCase);

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
        _mapperConfig = mapperConfig;
        _redis = redis.GetDatabase();
    }

    public static Dictionary<string, IXLWorksheets> RegisteredWorksheets { get; } =
        new(StringComparer.OrdinalIgnoreCase);

    public static Type? GetEntityType(string entityTypeName, string? fileName = null)
    {
        if (string.IsNullOrWhiteSpace(entityTypeName))
        {
            Console.WriteLine("⚠️ Entity type name cannot be null or empty.");
            return null;
        }

        var pascalName = char.ToUpper(entityTypeName[0]) + entityTypeName[1..].ToLower();
        var fullTypeName = $"{SpectrailConstants.DynamicAssemblyName}.{pascalName}Entity";

        var dynamicEntitiesPath = Path.Combine(AppContext.BaseDirectory, "DynamicEntities");

        if (!Directory.Exists(dynamicEntitiesPath))
        {
            Console.WriteLine("❌ DynamicEntities directory not found.");
            return null;
        }

        var loadedTypes = new List<Type>();
        string[] dllFiles;

        if (!string.IsNullOrWhiteSpace(fileName))
        {
            // 🔍 Normalize and locate specific DLL by filename
            var normalizedFileName = fileName.GetFileNameWithoutExtension()
                .Replace(" ", "", StringComparison.OrdinalIgnoreCase).Trim();
            dllFiles = Directory.GetFiles(dynamicEntitiesPath, "*.dll", SearchOption.TopDirectoryOnly)
                .Where(dll =>
                {
                    var segments = Path.GetFileNameWithoutExtension(dll)
                        .Split('.', StringSplitOptions.RemoveEmptyEntries);

                    var segmentToMatch = segments.Length >= 2
                        ? segments[^2]
                        : segments[^1];

                    return segmentToMatch.Replace(" ", "", StringComparison.OrdinalIgnoreCase)
                        .Equals(normalizedFileName, StringComparison.OrdinalIgnoreCase);
                })
                .ToArray();
        }
        else
        {
            // 🔁 Load all if filename not specified
            dllFiles = Directory.GetFiles(dynamicEntitiesPath, "*.dll", SearchOption.TopDirectoryOnly);
        }

        foreach (var dllPath in dllFiles)
            try
            {
                var assembly = Assembly.LoadFrom(dllPath);
                var types = assembly.GetTypes()
                    .Where(t => t.IsClass && t.Namespace == SpectrailConstants.DynamicAssemblyName)
                    .ToList();

                loadedTypes.AddRange(types);
                Console.WriteLine($"✅ Loaded {types.Count} types from {Path.GetFileName(dllPath)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error loading types from {dllPath}: {ex.Message}");
            }

        return loadedTypes.FirstOrDefault(t => t.FullName == fullTypeName);
    }

    private static (List<string> allEquipments,
        ConcurrentDictionary<string, List<IXLWorksheet>>
        sheetMap)
        ExtractEquipmentInfo(string filePath)
    {
        using var workbook = new XLWorkbook(filePath);
        var networkSheet = workbook.Worksheet(SpectrailConstants.ICD_NetworkConfig)
                           ?? throw new InvalidOperationException("⚠️ Sheet 'network_config' not found.");

        var headerRow = networkSheet.Row(1).Cells().Select(c => c.GetString().Trim()).ToList();
        var colIndex = headerRow.IndexOf("Equipment Sheet");
        if (colIndex == -1) throw new InvalidOperationException("⚠️ Column 'Equipment Sheet' not found.");

        var allEquipments = networkSheet.Column(colIndex + 1)
            .CellsUsed().Skip(1).Select(c => c.GetString().Trim())
            .Distinct().ToList();

        foreach (var worksheet in workbook.Worksheets)
        {
            var fileKey = Path.GetFileNameWithoutExtension(filePath);

            // Ensure thread-safe add or update
            icdWorksheetMap.AddOrUpdate(
                fileKey,
                _ => [worksheet.CopyTo(new XLWorkbook())],
                (_, existingList) =>
                {
                    existingList.Add(worksheet.CopyTo(new XLWorkbook()));
                    return existingList;
                });
        }


        //_registeredEntityByFile[filePath.GetFileNameWithoutExtension()] = equipmentsToRegister;
        return (allEquipments, icdWorksheetMap);
    }

    private static (List<Type> AllEquipmentEntity, List<Type> RegisteredEquipmentEntity)
        ProcessExcelAndRegisterEntitiesAndExtractEquipmentNames(string filePath,
            List<string>? equipmentsToRegister = null)
    {
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
        var allEquipmentEntity = new List<Type>();
        var registeredEquipmentEntity = new List<Type>();
        var (allEquipments, icdWorkSheets) = ExtractEquipmentInfo(filePath);
        var workSheets = icdWorkSheets[fileNameWithoutExtension];

        if (workSheets.Count == 0)
        {
            Console.WriteLine($"⚠️ No worksheets found in file: {filePath}");
            return ([], []);
        }

        try
        {
            var fileHashKey = $"{SpectrailConstants.RedisFileHashKey}{fileNameWithoutExtension}";
            var storedHash = _redis?.StringGet(fileHashKey).ToString();
            var currentHash = filePath.ComputeFileHash();

            if (storedHash != currentHash)
            {
                Console.WriteLine($"🧊 File Changed: {filePath.GetFileName()}. Skipping processing.");
                return ([], []);
            }

            var equipmentKey = $"{SpectrailConstants.RedisEquipmentHashKey}{fileNameWithoutExtension}";
            _redis?.KeyDelete(equipmentKey); // Clear previous

            foreach (var equipment in allEquipments)
                _redis?.HashSet(equipmentKey, equipment, equipmentsToRegister?.Contains(equipment).ToString());

            foreach (var worksheet in workSheets)
            {
                var sheetName = worksheet.Name.Trim().Replace(" ", "").ToUpper();
                var isRegistered = equipmentsToRegister?.Contains(sheetName, StringComparer.OrdinalIgnoreCase);
                var networkSheet = allEquipments.Contains(sheetName, StringComparer.OrdinalIgnoreCase);
                if (!networkSheet) continue;
                var entityType = GetEntityType(sheetName, fileNameWithoutExtension) ?? GenerateDynamicEntity(sheetName);
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

                if (isRegistered != null && isRegistered.Value) registeredEquipmentEntity.Add(entityType);

                allEquipmentEntity.Add(entityType);
            }

            DynamicEntityCompiler.CompileAndLoadEntities(allEquipmentEntity, fileNameWithoutExtension,
                registerDynamicEquipmentTypes =>
                {
                    var json = JsonSerializer.Serialize(registerDynamicEquipmentTypes);
                    _redis?.StringSetAsync(SpectrailConstants.RedisDynamicAssemblyCreated, json);
                });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error processing file: {filePath}, Msg: {ex.Message}");
        }

        return (allEquipmentEntity, registeredEquipmentEntity);
    }

    public List<Type> RegisterEntity(List<(string FileName, List<string> newEntities)> newEntitiesToRegister)
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

                var newEquipment = newEntitiesToRegister
                    .FirstOrDefault(m =>
                        m.FileName.GetFileNameWithoutExtension().Replace(" ", "", StringComparison.OrdinalIgnoreCase)
                            .Trim()
                            .Equals(filePath.GetFileNameWithoutExtension(), StringComparison.OrdinalIgnoreCase))
                    .newEntities;

                if (newEquipment.Count == 0) continue;

                var entityTypes = ProcessExcelAndRegisterEntitiesAndExtractEquipmentNames(filePath, newEquipment);

                if (entityTypes.AllEquipmentEntity.Count == 0)
                {
                    Console.WriteLine($"⚠️ No entity types found in file: {filePath}");
                    continue;
                }

                if (entityTypes.RegisteredEquipmentEntity.Count == 0)
                {
                    Console.WriteLine($"⚠️ No entity types registered: {filePath}");
                    continue;
                }

                registeredEntityTypes.AddRange(entityTypes.RegisteredEquipmentEntity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error processing file: {filePath}, Msg: {ex.Message}");
            }

        //_ = PersistRegisteredEntitiesAsync();
        return registeredEntityTypes.Distinct().ToList(); // ensure no duplicates
    }

    /*
    private static async Task PersistRegisteredEntitiesAsync()
    {
        var json = JsonSerializer.Serialize(_registeredEntityByFile);
        await _redis.StringSetAsync(SpectrailConstants.RedisEntityListKey, json);
    }

    public static async Task<Dictionary<string, List<string>>> LoadRegisteredEntitiesAsync()
    {
        var json = await _redis.StringGetAsync(SpectrailConstants.RedisEntityListKey);
        if (!json.IsNullOrEmpty)
            _registeredEntityByFile = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json)
                                      ?? new Dictionary<string, List<string>>();
        return _registeredEntityByFile;
    }


    public static List<string>? ExtractEquipmentNames(string filePath)
    {
        var fileName = Path.GetFileNameWithoutExtension(filePath).ToLower();
        if (_registeredEntityByFile.Count == 0) _ = LoadRegisteredEntitiesAsync();
        _registeredEntityByFile.TryGetValue(fileName, out var registeredEntity);
        return registeredEntity is { Count: > 0 } ? registeredEntity : null;
    }*/

    public static List<EntityMapping> GetAllMappings()
    {
        return _collection.Find(_ => true).ToList() ?? [];
    }

    public static async Task<List<EntityMapping>> GetEquipmentMappingsByFile(string fileName)
    {
        var filter = Filter.Eq(x => x.FileName, fileName);
        var result = await _collection.Find(filter).ToListAsync();
        return result;
    }

    public static async Task<List<EntityMapping>> GetRegisteredEquipmentMappingsByFile(string fileName)
    {
        var builder = Filter;
        var filter = builder.And(
            builder.Eq(x => x.FileName, fileName),
            builder.Eq(x => x.IsRegistered, true)
        );
        var result = await _collection.Find(filter).ToListAsync();
        return result;
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
        Console.WriteLine($"🛠 Generated and cached dynamic entity: {fullTypeName}");
        return dynamicType;
    }
}