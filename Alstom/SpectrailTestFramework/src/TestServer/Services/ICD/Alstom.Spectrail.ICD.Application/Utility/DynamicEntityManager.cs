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
// FileName: DynamicEntityManager.cs
// ProjectName: Alstom.Spectrail.ICD.Application
// Created by SUBHASISH BISWAS On: 2025-03-26
// Updated by SUBHASISH BISWAS On: 2025-03-26
//  ******************************************************************************/

#endregion

#region

using System.Collections.Concurrent;
using System.Reflection;
using Alstom.Spectrail.ICD.Application.Registry;
using Alstom.Spectrail.Server.Common.Entities;

#endregion

namespace Alstom.Spectrail.ICD.Application.Utility;

public static class DynamicEntityManager
{
    private static readonly ConcurrentDictionary<string, List<Type>> _entityTypeCache = new();

    public static async Task<List<Type>> LoadOrRegisterEntitiesAsync(IEnumerable<string> fileNames)
    {
        var normalizedFiles = fileNames
            .Select(f => f.GetFileNameWithoutExtension().ToLowerInvariant().Trim())
            .Distinct()
            .ToList();

        var loadedTypes = new List<Type>();
        var dynamicEntitiesPath = Path.Combine(AppContext.BaseDirectory, "DynamicEntities");

        var dllDirectoryExists = Directory.Exists(dynamicEntitiesPath);

        foreach (var file in normalizedFiles)
        {
            if (_entityTypeCache.TryGetValue(file, out var cachedTypes))
            {
                loadedTypes.AddRange(cachedTypes);
                continue;
            }

            if (!dllDirectoryExists)
            {
                Console.WriteLine("⚠️ DynamicEntities directory not found. Triggering fallback generation...");
                var generated = EntityRegistry.RegisterEntity([file]);
                _entityTypeCache[file] = generated;
                loadedTypes.AddRange(generated);
                continue;
            }

            var dllPath = Directory
                .EnumerateFiles(dynamicEntitiesPath, "*.dll")
                .FirstOrDefault(p =>
                    p.GetFileNameWithoutExtension().Split('.', StringSplitOptions.RemoveEmptyEntries)
                        .Reverse()
                        .Skip(1)
                        .FirstOrDefault()
                        ?.Equals(file, StringComparison.OrdinalIgnoreCase) == true);

            if (dllPath != null && File.Exists(dllPath))
                try
                {
                    var assembly = Assembly.LoadFrom(dllPath);
                    var allTypes = assembly.GetTypes()
                        .Where(t => t.IsClass &&
                                    t.Namespace == SpectrailConstants.DynamicAssemblyName &&
                                    typeof(EntityBase).IsAssignableFrom(t))
                        .ToList();

                    var registeredMappings = await EntityRegistry.GetRegisteredEquipmentMappingsByFile($"{file}.XLSX");
                    var registeredNames = registeredMappings
                        .Select(x => $"{char.ToUpper(x.SheetName[0])}{x.SheetName[1..]}Entity")
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var matchedTypes = allTypes.Where(t => registeredNames.Contains(t.Name)).ToList();
                    _entityTypeCache[file] = matchedTypes;
                    loadedTypes.AddRange(matchedTypes);
                    continue;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error loading from {dllPath}: {ex.Message}");
                }

            // DLL not found or failed to load, fallback to generation
            var fallbackGenerated = EntityRegistry.RegisterEntity([file]);
            _entityTypeCache[file] = fallbackGenerated;
            loadedTypes.AddRange(fallbackGenerated);
        }

        return loadedTypes;
    }

    public static Type? GetEntityType(string entityName, string? fileName = null)
    {
        if (string.IsNullOrWhiteSpace(entityName))
            return null;

        var pascalName = char.ToUpper(entityName[0]) + entityName[1..].ToLower();
        var fullTypeName = $"{SpectrailConstants.DynamicAssemblyName}.{pascalName}Entity";
        var key = fileName?.GetFileNameWithoutExtension().ToLowerInvariant().Trim() ?? "ALL";

        return _entityTypeCache.TryGetValue(key, out var types)
            ? types.FirstOrDefault(t => t.FullName == fullTypeName)
            : null;
    }
}