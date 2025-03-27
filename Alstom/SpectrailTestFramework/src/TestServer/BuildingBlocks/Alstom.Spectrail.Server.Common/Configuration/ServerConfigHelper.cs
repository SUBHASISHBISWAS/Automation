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
// FileName: ServerConfigHelper.cs
// ProjectName: Alstom.Spectrail.Server.Common
// Created by SUBHASISH BISWAS On: 2025-03-11
// Updated by SUBHASISH BISWAS On: 2025-03-28
//  ******************************************************************************/

#endregion

#region

using System.Reflection;
using Microsoft.Extensions.Configuration;

#endregion

namespace Alstom.Spectrail.Server.Common.Configuration;

/// <summary>
///     ✅ ServerConfigHelper dynamically reads configuration settings
///     using strongly typed `ICDConfig` and `FeatureFlagsConfig`.
///     Implements `IServerConfigHelper` for better testability.
/// </summary>
public class ServerConfigHelper : IServerConfigHelper
{
    private readonly IConfiguration _configuration;
    private readonly FeatureFlagsConfig _featureFlagsConfig;

    /// <summary>
    ///     ✅ Supports Dependency Injection (Pass IConfiguration)
    /// </summary>
    public ServerConfigHelper(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        _configuration = configuration;
        // ✅ Bind the "Settings" section to `ICDConfig`
        ICDConfig = configuration.GetSection("Settings").Get<ICDConfig>()
                    ?? throw new Exception("❌ Missing `Settings` section in appsettings.json!");

        // ✅ Bind the "FeatureFlags" section to `FeatureFlagsConfig`
        _featureFlagsConfig = configuration.GetSection("FeatureFlags").Get<FeatureFlagsConfig>()
                              ?? new FeatureFlagsConfig(); // Defaults to false if missing

        // ✅ Bind the "FeatureFlags" section to `FeatureFlagsConfig`
        SpectrailMongoConfig = configuration.GetSection("SpectrailMongoDatabaseSettings")
                                   .Get<SpectrailMongoDatabaseConfig>()
                               ?? new SpectrailMongoDatabaseConfig();

        SpectrailRedisConfig = configuration.GetSection("RedisConfig")
                                   .Get<SpectrailRedisConfiguration>()
                               ?? new SpectrailRedisConfiguration(); // Defaults to false if missing
    }

    public SpectrailRedisConfiguration SpectrailRedisConfig { get; }

    public SpectrailMongoDatabaseConfig SpectrailMongoConfig { get; }

    public ICDConfig ICDConfig { get; }


    /// <inheritdoc />
    public string GetICDFolderPath()
    {
        if (string.IsNullOrEmpty(ICDConfig.ICD_FOLDER_PATH))
            throw new Exception("❌ ICD_FOLDER_PATH is missing in appsettings.json!");

        var resolvedPath = ResolvePath(ICDConfig.ICD_FOLDER_PATH);
        if (!Directory.Exists(resolvedPath))
            throw new DirectoryNotFoundException($"❌ ICD folder not found: {resolvedPath}");

        return resolvedPath;
    }

    /// <inheritdoc />
    public List<string> GetICDFiles()
    {
        var icdFolderPath = GetICDFolderPath();
        var files = Directory.GetFiles(icdFolderPath, "*.xlsx")
            .Where(file => !Path.GetFileName(file).StartsWith("~$")) // Ignore temp files
            .ToList();

        if (files.Count == 0)
            throw new FileNotFoundException($"❌ No ICD Excel files found in {icdFolderPath}");

        return files;
    }

    /// <inheritdoc />
    public bool IsFeatureEnabled(string feature)
    {
        // ✅ Use reflection to safely retrieve feature flag values
        var property = typeof(FeatureFlagsConfig).GetProperty(feature,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        return property != null && (bool)property.GetValue(_featureFlagsConfig);
    }

    // ✅ Use this for simple keys from ICDConfig (flat)
    public T? GetSetting<T>(string key)
    {
        try
        {
            var setting = typeof(ICDConfig).GetProperty(key,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            return setting != null ? (T)setting.GetValue(ICDConfig) : default;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Error fetching setting '{key}': {ex.Message}");
            return default;
        }
    }

    // ✅ Use this for nested configuration (like JSON paths)
    public T GetSection<T>(string sectionKey)
    {
        try
        {
            return _configuration.GetSection(sectionKey).Get<T>() ?? throw new InvalidOperationException();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Error getting config section '{sectionKey}': {ex.Message}");
            return default;
        }
    }

    /// <summary>
    ///     ✅ Converts relative paths to absolute paths for cross-platform compatibility.
    /// </summary>
    private static string ResolvePath(string path)
    {
        if (string.IsNullOrEmpty(path)) return path;

        // ✅ If already an absolute path, return it
        if (Path.IsPathRooted(path))
            return path;

        // ✅ Determine base path based on OS
        var basePath = OperatingSystem.IsWindows()
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpectrailArtifacts")
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "SpectrailArtifacts");

        return Path.Combine(basePath, path);
    }
}