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
// Updated by SUBHASISH BISWAS On: 2025-03-27
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
        var fileList = fileNames?.ToList() ?? [];
        var loadedTypes = new List<Type>();
        var dynamicEntitiesPath = Path.Combine(AppContext.BaseDirectory, "DynamicEntities");

        Directory.CreateDirectory(dynamicEntitiesPath); // ensure folder exists

        foreach (var fullPath in fileList)
        {
            var fileKey = fullPath.GetFileNameWithoutExtension().ToLowerInvariant().Trim();

            // Skip if already cached
            if (_entityTypeCache.TryGetValue(fileKey, out var cached))
            {
                loadedTypes.AddRange(cached);
                continue;
            }

            // Try finding the DLL for this file
            var dllPath = Directory
                .EnumerateFiles(dynamicEntitiesPath, "*.dll", SearchOption.TopDirectoryOnly)
                .FirstOrDefault(p =>
                {
                    var segments = Path.GetFileNameWithoutExtension(p)
                        .Split('.', StringSplitOptions.RemoveEmptyEntries);

                    var nameSegment = segments.Length >= 2 ? segments[^2] : segments[^1];
                    return nameSegment.Equals(fileKey, StringComparison.OrdinalIgnoreCase);
                });

            if (!string.IsNullOrWhiteSpace(dllPath) && File.Exists(dllPath))
                try
                {
                    // 🔁 DLL found → just load from it
                    var registeredMappings =
                        await EntityRegistry.GetRegisteredEquipmentMappingsByFile($"{fileKey}.XLSX");
                    var registeredNames = registeredMappings
                        .Select(x => $"{char.ToUpper(x.SheetName[0])}{x.SheetName[1..]}Entity")
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var types = Assembly.LoadFrom(dllPath)
                        .GetTypes()
                        .Where(t =>
                            t.IsClass &&
                            t.Namespace == SpectrailConstants.DynamicAssemblyName &&
                            registeredNames.Contains(t.Name) &&
                            typeof(EntityBase).IsAssignableFrom(t))
                        .ToList();

                    _entityTypeCache[fileKey] = types;
                    loadedTypes.AddRange(types);
                    Console.WriteLine($"✅ Loaded {types.Count} types from DLL for {fileKey}");
                    continue;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error loading DLL from {dllPath}: {ex.Message}");
                    // Fallback to registration
                }

            // ❌ No DLL found or error → register and generate
            try
            {
                var registered = EntityRegistry.RegisterEntity([fullPath]);
                _entityTypeCache[fileKey] = registered;
                loadedTypes.AddRange(registered);
                Console.WriteLine($"🛠️ Registered {registered.Count} new types for {fileKey}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Registration failed for {fileKey}: {ex.Message}");
            }
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