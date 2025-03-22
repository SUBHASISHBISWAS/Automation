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
// Updated by SUBHASISH BISWAS On: 2025-03-22
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
    public static string ComputeFolderHash(this string folderPath, Func<string, string, Task>? getFileHashAsync = null)
    {
        if (!Directory.Exists(folderPath)) return string.Empty;

        using var sha = SHA256.Create();
        var files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories).OrderBy(f => f);

        var combinedHash = new StringBuilder();
        foreach (var file in files) combinedHash.Append(file.ComputeFileHash(getFileHashAsync));

        return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(combinedHash.ToString())))
            .Replace("-", "").ToLower();
    }

    public static string ComputeFileHash(this string filePath, Func<string, string, Task>? getFileHashAsync = null)
    {
        using var sha = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hashBytes = sha.ComputeHash(stream);
        var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        getFileHashAsync?.Invoke(hash, filePath);
        return hash;
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
}