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
// Updated by SUBHASISH BISWAS On: 2025-03-27
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Command;
using Alstom.Spectrail.ICD.Application.Utility;
using Alstom.Spectrail.Server.Common.Configuration;
using Alstom.Spectrail.Server.Common.Contracts;
using Autofac;
using MediatR;
using StackExchange.Redis;

#endregion

namespace Alstom.Spectrail.ICD.API.Middleware;

public class EntityRegistryOrchestrator(
    IMediator mediator,
    IConnectionMultiplexer redis,
    IServerConfigHelper configHelper,
    ILifetimeScope rootScope,
    IDynamicEntityLoader dynamicEntityLoader)
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
        if (changedFiles.Count == 0) return;
        if (loadedDynamicTypes.Count > 0)
        {
            /*rootScope.BeginLifetimeScope(builder =>
                DynamicRepositoryRegistrar.RegisterRepositoryHandlers(builder, loadedDynamicTypes));

            foreach (var entityType in loadedDynamicTypes)
                EntityRegistry.MapperConfig.CreateMap(entityType, typeof(CustomColumnDto)).ReverseMap();*/

            if (hasFolderChanged)
            {
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
        return await dynamicEntityLoader.LoadOrRegisterEntitiesAsync(icdFiles);
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