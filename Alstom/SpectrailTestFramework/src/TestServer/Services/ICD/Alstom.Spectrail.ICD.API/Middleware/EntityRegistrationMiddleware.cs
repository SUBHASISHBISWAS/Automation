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
// FileName: EntityRegistrationMiddleware.cs
// ProjectName: Alstom.Spectrail.ICD.API
// Created by SUBHASISH BISWAS On: 2025-03-20
// Updated by SUBHASISH BISWAS On: 2025-03-20
//  ******************************************************************************/

#endregion

#region

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Command;
using Alstom.Spectrail.ICD.Application.Registry;
using Alstom.Spectrail.Server.Common.Configuration;
using Alstom.Spectrail.Server.Common.Entities;
using MediatR;
using Newtonsoft.Json;
using StackExchange.Redis;

#endregion

namespace Alstom.Spectrail.ICD.API.Middleware;

/// <summary>
///     ✅ Middleware that combines `EntityRegistrationMiddleware` and `ICDSeedDataMiddleware`.
///     ✅ Detects both **file changes** and **new entity additions**.
///     ✅ Ensures **MongoDB schema is up-to-date** without redundant executions.
///     ✅ Uses **Redis** for caching previous states.
/// </summary>
public class EntityRegistrationMiddleware
{
    private readonly string _folderPath;
    private readonly RequestDelegate _next;
    private readonly IDatabase _redisDb;
    private readonly string _redisKey_EntityList = "RegisteredEntities";

    private readonly string _redisKey_FolderHash = "LastFolderHash";
    private readonly string _redisKey_RegistryCompleted = "EntityRegistryCompleted";
    private readonly IServiceScopeFactory _scopeFactory;

    public EntityRegistrationMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory,
        IServerConfigHelper configHelper, IConnectionMultiplexer redis)
    {
        _next = next;
        _scopeFactory = scopeFactory;
        _redisDb = redis.GetDatabase();
        _folderPath = configHelper.GetICDFolderPath();
    }

    public async Task Invoke(HttpContext context)
    {
        var hasFolderChanged = await HasFolderChanged();
        var hasNewEntities = await HasNewEntities();
        var isRegistryCompleted = await _redisDb.StringGetAsync(_redisKey_RegistryCompleted) == "true";
        using var scope = _scopeFactory.CreateScope();
        var entityRegistry = scope.ServiceProvider.GetRequiredService<EntityRegistry>();

        if ((hasNewEntities || hasFolderChanged) && !isRegistryCompleted)
        {
            Debug.WriteLine("🚀 Changes detected! Running entity registration and data seeding...");


            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            try
            {
                // ✅ Step 1: Register Entities (Only if needed)
                if (hasNewEntities)
                {
                    Debug.WriteLine("🔍 New entities detected. Registering...");
                    entityRegistry.RegisterEntity();
                }

                // ✅ Step 2: Seed MongoDB Data
                var success = await mediator.Send(new SeedICDDataCommand());

                if (success)
                {
                    await _redisDb.StringSetAsync(_redisKey_RegistryCompleted, "true", TimeSpan.FromHours(12));
                    Debug.WriteLine("✅ MongoDB Seeding Completed!");
                }
                else
                {
                    Debug.WriteLine("⚠️ MongoDB Seeding Failed!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error in Middleware: {ex.Message}");
            }
        }
        else
        {
            Debug.WriteLine("✅ No changes detected. Skipping registration.");
        }

        await _next(context);
    }

    /// <summary>
    ///     ✅ Checks if folder contents have changed using Redis cache.
    /// </summary>
    private async Task<bool> HasFolderChanged()
    {
        string? lastHash = await _redisDb.StringGetAsync(_redisKey_FolderHash);
        var currentHash = ComputeFolderHash(_folderPath);

        if (lastHash == currentHash) return false;

        await _redisDb.StringSetAsync(_redisKey_FolderHash, currentHash);
        return true;
    }

    /// <summary>
    ///     ✅ Detects if new entities have been added dynamically.
    /// </summary>
    private async Task<bool> HasNewEntities()
    {
        string? lastRegisteredEntitiesJson = await _redisDb.StringGetAsync(_redisKey_EntityList);
        var lastRegisteredEntities = lastRegisteredEntitiesJson != null
            ? JsonConvert.DeserializeObject<HashSet<string>>(lastRegisteredEntitiesJson)
            : new HashSet<string>();

        var currentEntities = GetAllEntityNames();

        if (!currentEntities.SetEquals(lastRegisteredEntities))
        {
            // ✅ Update Redis with the latest entity list
            var newEntityListJson = JsonConvert.SerializeObject(currentEntities);
            await _redisDb.StringSetAsync(_redisKey_EntityList, newEntityListJson);
            return true;
        }

        return false;
    }

    /// <summary>
    ///     ✅ Retrieves all dynamically registered entity names.
    /// </summary>
    private HashSet<string> GetAllEntityNames()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(asm => asm.FullName!.StartsWith("Alstom.Spectrail"))
            .ToList();

        var entityNames = new HashSet<string>();

        foreach (var asm in assemblies)
        {
            var entityTypes = asm.GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(EntityBase).IsAssignableFrom(t))
                .Select(t => t.FullName!)
                .ToList();

            entityNames.UnionWith(entityTypes);
        }

        return entityNames;
    }

    /// <summary>
    ///     ✅ Computes SHA256 hash for all files in the given folder.
    /// </summary>
    private string ComputeFolderHash(string folderPath)
    {
        if (!Directory.Exists(folderPath))
            return string.Empty;

        using var sha256 = SHA256.Create();
        var files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories)
            .OrderBy(f => f) // Ensure consistent order
            .ToList();

        var hashStringBuilder = new StringBuilder();

        foreach (var file in files)
        {
            using var stream = File.OpenRead(file);
            var hashBytes = sha256.ComputeHash(stream);
            hashStringBuilder.Append(BitConverter.ToString(hashBytes).Replace("-", "").ToLower());
        }

        var finalHashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(hashStringBuilder.ToString()));
        return BitConverter.ToString(finalHashBytes).Replace("-", "").ToLower();
    }
}