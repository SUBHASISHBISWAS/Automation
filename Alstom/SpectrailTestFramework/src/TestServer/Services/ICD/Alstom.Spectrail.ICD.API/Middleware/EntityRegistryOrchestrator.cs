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
// FileName: EntityRegistryOrchestrator.cs
// ProjectName: Alstom.Spectrail.ICD.API
// Created by SUBHASISH BISWAS On: 2025-03-21
// Updated by SUBHASISH BISWAS On: 2025-03-26
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Command;
using Alstom.Spectrail.ICD.Application.Registry;
using Alstom.Spectrail.ICD.Application.Utility;
using Alstom.Spectrail.ICD.Domain.DTO.ICD;
using Alstom.Spectrail.Server.Common.Configuration;
using Autofac;
using MediatR;
using StackExchange.Redis;

#endregion

namespace Alstom.Spectrail.ICD.API.Middleware;

public class EntityRegistryOrchestrator(
    IMediator mediator,
    IConnectionMultiplexer redis,
    IServerConfigHelper configHelper,
    ILifetimeScope rootScope)
{
    private const string RedisKeyEntityList = "RegisteredEntities";

    private const string RedisKeyRegistryCompleted = "EntityRegistryCompleted";
    private readonly IDatabase _redisDb = redis.GetDatabase();

    public async Task ExecuteAsync(bool force = false)
    {
        var folderPath = configHelper.GetICDFolderPath();
        var hasFolderChanged = await HasFolderChanged(folderPath);
        var changedFiles = await configHelper.GetICDFiles().GetAndStoreChangedFilesFromRedis(
            async key => (await _redisDb.StringGetAsync(key)).ToString(),
            async (key, value) => await _redisDb.StringSetAsync(key, value)
        );
        var loadedDynamicTypes = await RegisterOrLoadExistingDynamicEntities(changedFiles);
        if (loadedDynamicTypes.Count > 0)
        {
            rootScope.BeginLifetimeScope(builder =>
                DynamicRepositoryRegistrar.RegisterRepositoryHandlers(builder, loadedDynamicTypes));

            foreach (var entityType in loadedDynamicTypes)
                EntityRegistry.MapperConfig.CreateMap(entityType, typeof(CustomColumnDto)).ReverseMap();

            if (hasFolderChanged)
            {
                /*changedFiles = await configHelper.GetICDFiles().GetAndStoreChangedFilesFromRedis(
                    async key => (await _redisDb.StringGetAsync(key)).ToString(),
                    async (key, value) => await _redisDb.StringSetAsync(key, value)
                );*/


                var seeded = await mediator.Send(new SeedICDDataCommand { ICDFiles = changedFiles });

                if (seeded)
                {
                    await _redisDb.StringSetAsync(RedisKeyRegistryCompleted, "true", TimeSpan.FromHours(12));
                    Console.WriteLine("✅ MongoDB Seeding Completed!");
                }
                else
                {
                    Console.WriteLine("⚠️ MongoDB Seeding Failed!");
                }
            }
            else
            {
                Console.WriteLine("✅ No entity or folder changes. Skipping.");
            }
        }
    }

    private async Task<List<Type>>
        RegisterOrLoadExistingDynamicEntities(List<string> icdFiles)
    {
        return await DynamicEntityManager.LoadOrRegisterEntitiesAsync(icdFiles);

        /*var dynamicEntitiesPath = Path.Combine(AppContext.BaseDirectory, "DynamicEntities");

        if (!Directory.Exists(dynamicEntitiesPath))
        {
            Console.WriteLine("❌ DynamicEntities directory not found.");
            return EntityRegistry.RegisterEntity(icdFiles);
        }

        var dllFiles = Directory.GetFiles(dynamicEntitiesPath, "*.dll", SearchOption.TopDirectoryOnly);
        var loadedTypes = new List<Type>();

        foreach (var dllPath in dllFiles)
        {
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(dllPath);

            var segments = fileNameWithoutExt
                .Split('.', StringSplitOptions.RemoveEmptyEntries);

            var normalizedFileName = segments.Length >= 2
                ? segments[^2].Replace(" ", "", StringComparison.OrdinalIgnoreCase).Trim()
                : fileNameWithoutExt.Replace(" ", "", StringComparison.OrdinalIgnoreCase).Trim();

            var fileKey = $"{normalizedFileName}.XLSX";

            try
            {
                var registeredMappings = await EntityRegistry.GetRegisteredEquipmentMappingsByFile(fileKey);
                if (registeredMappings.Count == 0)
                {
                    Console.WriteLine($"⚠️ No registered entities found for: {fileKey}");
                    continue;
                }

                var registeredSheetNames = registeredMappings
                    .Select(x => $"{char.ToUpper(x.SheetName[0])}{x.SheetName[1..]}Entity")
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                var assembly = Assembly.LoadFrom(dllPath);
                var types = assembly.GetTypes()
                    .Where(t =>
                        t.IsClass &&
                        t.Namespace == SpectrailConstants.DynamicAssemblyName &&
                        registeredSheetNames.Contains(t.Name))
                    .ToList();

                // ⚠️ Mismatch in count
                if (types.Count != registeredSheetNames.Count)
                    Console.WriteLine(
                        $"⚠️ Partial load: Only {types.Count}/{registeredSheetNames.Count} types loaded from {fileKey}");

                loadedTypes.AddRange(types);
                Console.WriteLine($"✅ Loaded {types.Count} registered types from {Path.GetFileName(dllPath)}");

                if (!registeredMappings.Any(m =>
                        m.FileName.GetFileNameWithoutExtension().Replace(" ", "", StringComparison.OrdinalIgnoreCase)
                            .Trim()
                            .Equals(normalizedFileName, StringComparison.OrdinalIgnoreCase)))
                    loadedTypes.AddRange(EntityRegistry.RegisterEntity(icdFiles));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error loading types from {dllPath}: {ex.Message}");
            }
        }

        return loadedTypes;*/
    }

    private async Task<bool> HasFolderChanged(string folderPath)
    {
        string? lastHash = await _redisDb.StringGetAsync(SpectrailConstants.RedisKeyFolderHash);
        var currentHash = folderPath.ComputeFolderHash();

        if (lastHash == currentHash)
            return false;

        await _redisDb.StringSetAsync(SpectrailConstants.RedisKeyFolderHash, currentHash);
        return true;
    }
}