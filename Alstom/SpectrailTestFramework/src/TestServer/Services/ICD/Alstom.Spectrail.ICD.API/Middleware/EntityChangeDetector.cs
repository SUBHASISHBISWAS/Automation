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
// FileName: EntityChangeDetector.cs
// ProjectName: Alstom.Spectrail.ICD.API
// Created by SUBHASISH BISWAS On: 2025-03-20
// Updated by SUBHASISH BISWAS On: 2025-03-20
//  ******************************************************************************/

#endregion

/*#region ©COPYRIGHT

// /*******************************************************************************
//  *   © COPYRIGHT ALSTOM SA 2025 - ALL RIGHTS RESERVED
//  *
//  *  This file is part of the Alstom Spectrail project.
//  *  Unauthorized copying, modification, distribution, or use of this file,
//  *  via any medium, is strictly prohibited.
//  *
//  *  Proprietary and confidential.
//  *  Created and maintained by Alstom for internal use in the Spectrail project.
//  *****************************************************************************#1#
//
//  /*******************************************************************************
// AuthorName: SUBHASISH BISWAS
// Email: subhasish.biswas@alstomgroup.com
// FileName: EntityChangeDetector.cs
// ProjectName: Alstom.Spectrail.ICD.API
// Created by SUBHASISH BISWAS On: 2025-03-20
// Updated by SUBHASISH BISWAS On: 2025-03-20
//  *****************************************************************************#1#

#endregion

#region

using System.Security.Cryptography;
using System.Text;
using Alstom.Spectrail.Server.Common.Configuration;
using Alstom.Spectrail.Server.Common.Entities;
using Newtonsoft.Json;
using StackExchange.Redis;

#endregion

namespace Alstom.Spectrail.ICD.API.Middleware;

/// <summary>
///     ✅ Utility class for detecting file changes and new entities.
///     ✅ Provides reusable Redis methods for both middlewares.
/// </summary>
public class EntityChangeDetector(IConnectionMultiplexer redis, IServerConfigHelper configHelper)
{
    private readonly string _folderHashKey = "LastFolderHash";
    private readonly string _folderPath = configHelper.GetICDFolderPath();
    private readonly IDatabase _redisDb = redis.GetDatabase();
    private readonly string _registeredEntitiesKey = "RegisteredEntities";

    /// <summary>
    ///     ✅ Checks if folder contents have changed by comparing hash values.
    /// </summary>
    public async Task<bool> HasFolderChangedAsync()
    {
        string? lastHash = await _redisDb.StringGetAsync(_folderHashKey);
        var currentHash = ComputeFolderHash(_folderPath);

        if (lastHash == currentHash) return false;

        await _redisDb.StringSetAsync(_folderHashKey, currentHash);
        return true;
    }

    /// <summary>
    ///     ✅ Detects if new entities have been added dynamically.
    /// </summary>
    public async Task<bool> HasNewEntitiesAsync()
    {
        string? lastRegisteredEntitiesJson = await _redisDb.StringGetAsync(_registeredEntitiesKey);
        var lastRegisteredEntities = lastRegisteredEntitiesJson != null
            ? JsonConvert.DeserializeObject<HashSet<string>>(lastRegisteredEntitiesJson)
            : new HashSet<string>();

        var currentEntities = GetAllEntityNames();

        if (!currentEntities.SetEquals(lastRegisteredEntities))
        {
            // ✅ Update Redis with the latest entity list
            var newEntityListJson = JsonConvert.SerializeObject(currentEntities);
            await _redisDb.StringSetAsync(_registeredEntitiesKey, newEntityListJson);
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
}*/
