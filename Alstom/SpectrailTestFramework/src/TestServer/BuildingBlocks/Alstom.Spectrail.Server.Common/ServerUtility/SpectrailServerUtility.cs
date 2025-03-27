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
// FileName: SpectrailServerUtility.cs
// ProjectName: Alstom.Spectrail.Server.Common
// Created by SUBHASISH BISWAS On: 2025-03-27
// Updated by SUBHASISH BISWAS On: 2025-03-28
//  ******************************************************************************/

#endregion

#region

using MongoDB.Driver;
using StackExchange.Redis;

#endregion

namespace Alstom.Spectrail.Server.Common.ServerUtility;

public static class SpectrailServerUtility
{
    public static async Task ResetSpectrailServerAsync(
        string? mongoConnectionString,
        IEnumerable<string> mongoDatabaseNames,
        string redisConnectionString, string redisPrefix, string dynamicFolderName)
    {
        try
        {
            Console.WriteLine("🔁 Starting Spectrail Server RESET...");

            // 1️⃣ Mongo Cleanup
            var mongoClient = new MongoClient(mongoConnectionString);
            foreach (var dbName in mongoDatabaseNames.Distinct(StringComparer.OrdinalIgnoreCase))
                try
                {
                    await mongoClient.DropDatabaseAsync(dbName);
                    Console.WriteLine($"🗑 Dropped MongoDB database: {dbName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Failed to drop MongoDB database '{dbName}': {ex.Message}");
                }

            // 2️⃣ Redis Cleanup
            var redis = await ConnectionMultiplexer.ConnectAsync(redisConnectionString);
            var redisDb = redis.GetDatabase();
            var server = redis.GetServer(redis.GetEndPoints().First());

            var keys = server.Keys(pattern: $"{redisPrefix}*").ToArray();
            if (keys.Length == 0)
                Console.WriteLine("✅ No Redis keys to delete.");
            else
                foreach (var key in keys)
                {
                    await redisDb.KeyDeleteAsync(key);
                    Console.WriteLine($"❌ Deleted Redis key: {key}");
                }

            // 3️⃣ Dynamic Folder Cleanup
            var dynamicPath = Path.Combine(AppContext.BaseDirectory, dynamicFolderName);
            if (Directory.Exists(dynamicPath))
            {
                Directory.Delete(dynamicPath, true);
                Console.WriteLine($"🧹 Deleted dynamic folder: {dynamicPath}");
            }
            else
            {
                Console.WriteLine($"ℹ️ Dynamic folder not found: {dynamicPath}");
            }

            Console.WriteLine("✅ Spectrail Server reset completed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Reset failed: {ex.Message}");
            throw;
        }
    }
}