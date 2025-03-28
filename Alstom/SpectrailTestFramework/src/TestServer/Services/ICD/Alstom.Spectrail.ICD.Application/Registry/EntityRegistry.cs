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
// Updated by SUBHASISH BISWAS On: 2025-03-27
//  ******************************************************************************/

#endregion

#region

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using Alstom.Spectrail.ICD.Application.Contracts;
using Alstom.Spectrail.ICD.Application.Models;
using Alstom.Spectrail.ICD.Application.Utility;
using Alstom.Spectrail.ICD.Domain.DTO.ICD;
using Alstom.Spectrail.ICD.Domain.Entities.ICD;
using Alstom.Spectrail.Server.Common.Contracts;
using Alstom.Spectrail.Server.Common.Entities;
using AutoMapper;
using ClosedXML.Excel;
using MongoDB.Bson;
using MongoDB.Driver;
using static MongoDB.Driver.Builders<Alstom.Spectrail.ICD.Application.Models.EntityMapping>;

#endregion

namespace Alstom.Spectrail.ICD.Application.Registry;

public class EntityRegistry
{
    private static IMongoCollection<EntityMapping> _collection;
    private static IDynamicEntityLoader _dynamicEntityLoader;

    public EntityRegistry(IICDDbContext dbContext, IDynamicEntityLoader dynamicEntityLoader,
        IMapperConfigurationExpression mapperConfig)
    {
        ArgumentNullException.ThrowIfNull(dbContext, nameof(IMongoCollection<EntityMapping>));
        ArgumentNullException.ThrowIfNull(mapperConfig, nameof(IMapperConfigurationExpression));

        Debug.Assert(dbContext.ICDEntityMapping != null);
        _collection = dbContext.ICDEntityMapping;
        _dynamicEntityLoader = dynamicEntityLoader;
        MapperConfig = mapperConfig;
    }

    public static ConcurrentDictionary<string, List<IXLWorksheet>> RegisteredWorksheets { get; } =
        new(StringComparer.OrdinalIgnoreCase);

    public static IMapperConfigurationExpression MapperConfig { get; private set; } = default!;

    public static Type? GetEntityType(string entityTypeName, string? fileName = null)
    {
        return _dynamicEntityLoader.GetEntityType(entityTypeName, fileName);
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
            .CellsUsed()
            .Skip(1)
            .Select(c => c.GetString().Trim())
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToHashSet();

        foreach (var worksheet in workbook.Worksheets)
        {
            var fileKey = Path.GetFileNameWithoutExtension(filePath);

            // Only process worksheet if it matches an equipment name
            if (!allEquipments.Contains(worksheet.Name.Trim()))
                continue;

            RegisteredWorksheets.AddOrUpdate(
                fileKey,
                _ => [worksheet.CopyTo(new XLWorkbook())],
                (_, existingList) =>
                {
                    existingList.Add(worksheet.CopyTo(new XLWorkbook()));
                    return existingList;
                });
        }

        return (allEquipments.ToList(), RegisteredWorksheets);
    }

    private static List<Type>?
        ProcessExcelAndRegisterEntities(string filePath)
    {
        List<Type>? compiledAndRegisteredEntities = null;
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
        var dynamicEntities = new List<Type>();
        var (allEquipments, icdWorkSheets) = ExtractEquipmentInfo(filePath);
        var workSheets = icdWorkSheets[fileNameWithoutExtension];

        if (workSheets.Count == 0)
        {
            Console.WriteLine($"⚠️ No worksheets found in file: {filePath}");
            return [];
        }

        try
        {
            foreach (var worksheet in workSheets)
            {
                var sheetName = worksheet.Name.Trim().Replace(" ", "").ToUpper();
                var isRegistered = allEquipments.Contains(sheetName, StringComparer.OrdinalIgnoreCase);
                var networkSheet = allEquipments.Contains(sheetName, StringComparer.OrdinalIgnoreCase);
                if (!networkSheet) continue;
                var entityType = GetEntityType(sheetName, fileNameWithoutExtension) ?? GenerateDynamicEntity(sheetName);

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

                if (isRegistered) dynamicEntities.Add(entityType);
            }

            //TODO: Check if the EquipmentName is not changed then skip the dll generationeration
            DynamicEntityCompiler.CompileAndSaveEntitiesToAssembly(dynamicEntities,
                fileNameWithoutExtension, compiledEntities =>
                {
                    compiledAndRegisteredEntities = compiledEntities;
                    foreach (var entityType in compiledEntities)
                        MapperConfig.CreateMap(entityType, typeof(CustomColumnDto)).ReverseMap();
                });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error processing file: {filePath}, Msg: {ex.Message}");
        }

        return compiledAndRegisteredEntities;
    }

    public static List<Type> RegisterEntity(IEnumerable<string> icdFiles)
    {
        var registeredEntityTypes = new List<Type>();
        foreach (var filePath in icdFiles)
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"⚠️ File not found: {filePath}");
                    continue;
                }

                var entityTypes = ProcessExcelAndRegisterEntities(filePath);


                if (entityTypes?.Count == 0)
                {
                    Console.WriteLine($"⚠️ No entity types registered: {filePath}");
                    continue;
                }

                Debug.Assert(entityTypes != null, nameof(entityTypes) + " != null");
                registeredEntityTypes.AddRange(entityTypes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error processing file: {filePath}, Msg: {ex.Message}");
            }

        return registeredEntityTypes.Distinct().ToList(); // ensure no duplicates
    }


    public static List<EntityMapping> GetAllMappings()
    {
        return _collection.Find(_ => true).ToList() ?? [];
    }

    public static async Task<List<EntityMapping>> GetRegisteredEquipmentMappingsByFile(string fileName)
    {
        var normalizedFileName = Path.GetFileNameWithoutExtension(fileName).Trim();

        var regexPattern = $"^{Regex.Escape(normalizedFileName)}(\\.xlsx)?$";
        var regex = new BsonRegularExpression(regexPattern, "i"); // case-insensitive match

        var builder = Filter; // 👈 Use `Builders<T>.Filter` not just `Filter`
        var filter = builder.And(
            builder.Regex(x => x.FileName, regex),
            builder.Eq(x => x.IsRegistered, true)
        );

        var result = await _collection.Find(filter).ToListAsync();
        return result;
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