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
// FileName: DynamicEntityLoaderService.cs
// ProjectName: Alstom.Spectrail.ICD.Application
// Created by SUBHASISH BISWAS On: 2025-03-27
// Updated by SUBHASISH BISWAS On: 2025-03-27
//  ******************************************************************************/

#endregion

#region

using System.Collections.Concurrent;
using System.Reflection;
using Alstom.Spectrail.ICD.Application.Registry;
using Alstom.Spectrail.ICD.Application.Utility;
using Alstom.Spectrail.ICD.Domain.DTO.ICD;
using Alstom.Spectrail.Server.Common.Contracts;
using Alstom.Spectrail.Server.Common.Entities;
using Autofac;

#endregion

namespace Alstom.Spectrail.ICD.Application.Services;

public class DynamicEntityLoaderService(ILifetimeScope rootScope) : IDynamicEntityLoader
{
    private readonly ConcurrentDictionary<string, List<Type>> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly string _dllDirectory = Path.Combine(AppContext.BaseDirectory, "DynamicEntities");

    public async Task<List<Type>> LoadOrRegisterEntitiesAsync(IEnumerable<string> fileNames)
    {
        var files = fileNames.ToList();
        var results = new List<Type>();
        var dllExists = Directory.Exists(_dllDirectory);

        if (!dllExists)
        {
            Console.WriteLine("⚠️ DLL directory missing. Registering all entities...");
            return RegisterAll(files);
        }

        var dllFiles = Directory.GetFiles(_dllDirectory, "*.dll", SearchOption.TopDirectoryOnly);

        // 🔁 Load ALL types from all DLLs, not just changed ones
        foreach (var dll in dllFiles)
        {
            var fileKey = dll.GetFileNameWithoutExtension()
                .Split('.', StringSplitOptions.RemoveEmptyEntries)
                .Reverse().Skip(1).FirstOrDefault()?.ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(fileKey))
                continue;

            if (_cache.TryGetValue(fileKey, out var cached))
            {
                results.AddRange(cached);
                continue;
            }

            var types = await LoadFromDllAsync(dll, fileKey);
            results.AddRange(types);
        }

        // 🔧 Register only the files that are actually new/changed
        foreach (var changedFile in files)
        {
            var key = changedFile.GetFileNameWithoutExtension().ToLowerInvariant();
            var dllExistsForFile = dllFiles.Any(d =>
                d.GetFileNameWithoutExtension().Contains($".{key}.", StringComparison.OrdinalIgnoreCase));

            if (dllExistsForFile) continue;
            Console.WriteLine($"🛠️ No DLL for changed file {key}, triggering registration...");
            var generated = EntityRegistry.RegisterEntity([changedFile]);
            _cache[key] = generated;
            results.AddRange(generated);
        }

        rootScope.BeginLifetimeScope(builder =>
            DynamicRepositoryRegistrar.RegisterRepositoryHandlers(builder, results));

        foreach (var entityType in results)
            EntityRegistry.MapperConfig.CreateMap(entityType, typeof(CustomColumnDto)).ReverseMap();
        return results;
    }

    public Type? GetEntityType(string entityName, string? fileName = null)
    {
        if (string.IsNullOrWhiteSpace(entityName))
        {
            Console.WriteLine("⚠️ Entity name is null or empty.");
            return null;
        }

        // Normalize entity name to PascalCase
        var pascalName = char.ToUpperInvariant(entityName[0]) + entityName[1..].ToLowerInvariant();
        var fullTypeName = $"{SpectrailConstants.DynamicAssemblyName}.{pascalName}Entity";

        // Use "ALL" key for unscoped lookups
        var key = string.IsNullOrWhiteSpace(fileName)
            ? "ALL"
            : fileName.GetFileNameWithoutExtension().ToLowerInvariant().Trim();

        if (_cache.TryGetValue(key, out var types))
            return types.FirstOrDefault(t => t.FullName == fullTypeName);

        Console.WriteLine($"❌ Entity type '{fullTypeName}' not found in cache for key: {key}");
        return null;
    }

    private async Task<List<Type>> LoadFromDllAsync(string dllPath, string fileKey)
    {
        try
        {
            var assembly = Assembly.LoadFrom(dllPath);
            var types = assembly.GetTypes()
                .Where(t => t.IsClass &&
                            t.Namespace == SpectrailConstants.DynamicAssemblyName &&
                            typeof(EntityBase).IsAssignableFrom(t))
                .ToList();

            var mappings = await EntityRegistry.GetRegisteredEquipmentMappingsByFile($"{fileKey}.XLSX");
            var registeredNames = mappings
                .Select(x => $"{char.ToUpper(x.SheetName[0])}{x.SheetName[1..]}Entity")
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var matched = types.Where(t => registeredNames.Contains(t.Name)).ToList();
            _cache[fileKey] = matched;

            return matched;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to load from {dllPath}: {ex.Message}");
            return [];
        }
    }

    private List<Type> RegisterAll(IEnumerable<string> filePaths)
    {
        var registered = new List<Type>();
        var files = filePaths.Any()
            ? filePaths
            : Directory.GetFiles(_dllDirectory, "*.dll");

        foreach (var file in files)
        {
            var key = file.GetFileNameWithoutExtension().ToLowerInvariant();
            var types = EntityRegistry.RegisterEntity([file]);
            _cache[key] = types;
            registered.AddRange(types);
        }

        return registered;
    }
}