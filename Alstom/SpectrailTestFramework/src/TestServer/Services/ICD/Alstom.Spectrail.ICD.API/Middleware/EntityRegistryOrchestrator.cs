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
// Updated by SUBHASISH BISWAS On: 2025-03-22
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Command;
using Alstom.Spectrail.ICD.Application.Registry;
using Alstom.Spectrail.ICD.Application.Utility;
using Alstom.Spectrail.Server.Common.Configuration;
using Alstom.Spectrail.Server.Common.Entities;
using Autofac;
using MediatR;
using Newtonsoft.Json;
using StackExchange.Redis;

#endregion

namespace Alstom.Spectrail.ICD.API.Middleware;

public class EntityRegistryOrchestrator(
    EntityRegistry entityRegistry,
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
        var hasNewEntities = await HasNewEntities();
        var isRegistryCompleted = await _redisDb.StringGetAsync(RedisKeyRegistryCompleted) == "true";

        if (hasFolderChanged || hasNewEntities || force || !isRegistryCompleted)
        {
            Console.WriteLine("🚀 Changes detected! Running entity registration and data seeding...");

            // 🔁 Register entities and get list of dynamic types
            var dynamicTypes = entityRegistry.RegisterEntity();


            // 🔄 Register handlers via Autofac dynamically
            rootScope.BeginLifetimeScope(builder =>
            {
                DynamicRepositoryRegistrar.RegisterRepositoryHandlers(builder, dynamicTypes);
            });

            var seeded = await mediator.Send(new SeedICDDataCommand());

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

    private async Task<bool> HasFolderChanged(string folderPath)
    {
        string? lastHash = await _redisDb.StringGetAsync(SpectrailConstants.RedisKeyFolderHash);
        var currentHash = folderPath.ComputeFolderHash(async (hash, filePath) =>
        {
            await _redisDb.StringSetAsync(
                $"{SpectrailConstants.RedisFileHashKey}{filePath.GetFileNameWithoutExtension()}",
                hash);
        });

        if (lastHash == currentHash)
            return false;

        await _redisDb.StringSetAsync(SpectrailConstants.RedisKeyFolderHash, currentHash);
        return true;
    }

    private async Task<bool> HasNewEntities()
    {
        string? lastJson = await _redisDb.StringGetAsync(RedisKeyEntityList);
        var lastEntities = lastJson != null
            ? JsonConvert.DeserializeObject<HashSet<string>>(lastJson)
            : new HashSet<string>();

        var currentEntities = GetAllEntityNames();

        if (!currentEntities.SetEquals(lastEntities))
        {
            await _redisDb.StringSetAsync(RedisKeyEntityList, JsonConvert.SerializeObject(currentEntities));
            return true;
        }

        return false;
    }

    private HashSet<string> GetAllEntityNames()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName!.StartsWith("Alstom.Spectrail"))
            .SelectMany(a => a.GetTypes())
            .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(EntityBase).IsAssignableFrom(t))
            .Select(t => t.FullName!)
            .ToHashSet();
    }
}