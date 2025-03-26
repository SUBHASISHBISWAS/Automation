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
// FileName: Utility.cs
// ProjectName: Alstom.Spectrail.ICD.Application
// Created by SUBHASISH BISWAS On: 2025-03-22
// Updated by SUBHASISH BISWAS On: 2025-03-26
//  ******************************************************************************/

#endregion

#region

using System.Reflection;
using System.Security.Cryptography;
using System.Text;

#endregion

namespace Alstom.Spectrail.ICD.Application.Utility;

public static class SpectralUtility
{
    public static string ComputeFolderHash(this string folderPath)
    {
        if (!Directory.Exists(folderPath)) return string.Empty;

        using var sha = SHA256.Create();
        var files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories).OrderBy(f => f);


        var combinedHash = new StringBuilder();
        foreach (var file in files) combinedHash.Append(file.ComputeFileHash());

        return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(combinedHash.ToString())))
            .Replace("-", "").ToLower();
    }

    public static string ComputeFileHash(this string filePath)
    {
        using var sha = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hashBytes = sha.ComputeHash(stream);
        var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        return hash;
    }

    public static async Task StoreFileHash(this string filePath, Func<string, string, Task> storeInRedis)
    {
        try
        {
            var computedHash = filePath.ComputeFileHash();
            var fileNameKey = filePath.GetFileNameWithoutExtension();
            var redisKey = $"{SpectrailConstants.RedisFileHashKey}{fileNameKey}";
            await storeInRedis(redisKey, computedHash);
        }
        catch (Exception e)
        {
            throw new Exception($"❌ Error storing file hash for {filePath}: {e.Message}");
        }
    }

    public static string GetFileNameWithoutExtension(this string filePath)
    {
        var fileName = Path.GetFileName(filePath).ToUpper();
        return Path.GetFileNameWithoutExtension(fileName);
    }

    public static string GetFileName(this string filePath)
    {
        return Path.GetFileName(filePath).ToUpper();
    }

    public static IEnumerable<Type> SafeGetTypes(this Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t != null)!;
        }
    }

    public static async Task<List<string>> GetChangedFilesFromRedis(this
            IEnumerable<string> filePaths,
        Func<string, Task<string?>> storeInRedis)
    {
        var changedFiles = new List<string>();

        foreach (var filePath in filePaths)
        {
            var fileNameKey = filePath.GetFileNameWithoutExtension();
            var redisKey = $"{SpectrailConstants.RedisFileHashKey}{fileNameKey}";

            // 🔁 Fetch previously stored hash from Redis via injected delegate
            var storedHash = await storeInRedis(redisKey);

            // 🧮 Compute current folder/file hash
            var currentHash = filePath.ComputeFileHash();

            // 🔍 Compare hashes and detect change
            if (string.IsNullOrWhiteSpace(storedHash) ||
                !string.Equals(storedHash, currentHash, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"🔄 File changed: {fileNameKey}");
                changedFiles.Add(filePath);
            }
            else
            {
                Console.WriteLine($"✅ File unchanged: {fileNameKey}");
            }
        }

        return changedFiles;
    }

    public static async Task<List<string>> GetAndStoreChangedFilesFromRedis(
        this IEnumerable<string> filePaths,
        Func<string, Task<string?>> fetchFromRedis,
        Func<string, string, Task> storeToRedis)
    {
        var changedFiles = new List<string>();

        foreach (var filePath in filePaths)
        {
            var fileNameKey = filePath.GetFileNameWithoutExtension();
            var redisKey = $"{SpectrailConstants.RedisFileHashKey}{fileNameKey}";

            string currentHash;
            try
            {
                currentHash = filePath.ComputeFileHash();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Could not compute hash for {filePath}: {ex.Message}");
                continue;
            }

            var storedHash = await fetchFromRedis(redisKey);

            if (string.IsNullOrWhiteSpace(storedHash) ||
                !string.Equals(storedHash, currentHash, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"🔄 File changed: {fileNameKey}");

                changedFiles.Add(filePath);

                try
                {
                    await storeToRedis(redisKey, currentHash);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"❌ Failed to update Redis for {fileNameKey}: {e.Message}");
                }
            }
            else
            {
                Console.WriteLine($"✅ File unchanged: {fileNameKey}");
            }
        }

        return changedFiles;
    }
}