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
// FileName: ConfigHelper.cs
// ProjectName: Alstom.Spectrail.TestFramework
// Created by SUBHASISH BISWAS On: 2025-03-13
// Updated by SUBHASISH BISWAS On: 2025-03-13
//  ******************************************************************************/

#endregion

#region

using Microsoft.Extensions.Configuration;

#endregion

namespace Alstom.Spectrail.TestFramework.Utilities;

public class ConfigHelper
{
    private readonly IConfigurationRoot _configuration;

    public ConfigHelper()
    {
        // ✅ Determine Base Path for Config Files (Cross-Platform)
        var basePath = Environment.GetEnvironmentVariable("CONFIG_PATH") // For Docker
                       ?? Directory.GetCurrentDirectory(); // For macOS, Windows, and Linux

        _configuration = new ConfigurationBuilder()
            .SetBasePath(basePath) // ✅ Dynamically determine the config path
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                true, true) // ✅ Support for Dev/Test/Prod
            .AddEnvironmentVariables() // ✅ Enables environment variable overrides
            .Build();
    }

    /// <summary>
    ///     ✅ Retrieves a setting from `appsettings.json`
    /// </summary>
    public string? GetSetting(string key)
    {
        if (string.IsNullOrEmpty(key)) throw new ArgumentException("Value cannot be null or empty.", nameof(key));

        return _configuration[$"PlaywrightSettings:{key}"]
               ?? _configuration[$"Settings:{key}"]
               ?? _configuration[$"ServerSettings:{key}"]
               ?? throw new Exception($"❌ Setting '{key}' not found in appsettings.json!");
    }

    /// <summary>
    ///     ✅ Retrieves a boolean setting (default = false)
    /// </summary>
    public bool GetBoolSetting(string key, bool defaultValue = false)
    {
        var value = _configuration[$"PlaywrightSettings:{key}"];
        return bool.TryParse(value, out var result) ? result : defaultValue;
    }

    /// <summary>
    ///     ✅ Retrieves an integer setting (default = 0)
    /// </summary>
    public int GetIntSetting(string key, int defaultValue = 0)
    {
        var value = _configuration[$"PlaywrightSettings:{key}"];
        return int.TryParse(value, out var result) ? result : defaultValue;
    }

    /// <summary>
    ///     ✅ Retrieves URLs from `Settings` section
    /// </summary>
    public string GetUrl(string key)
    {
        return _configuration[$"Settings:{key}"]
               ?? throw new Exception($"❌ URL '{key}' not found in appsettings.json!");
    }

    /// <summary>
    ///     ✅ Retrieves an entire configuration section
    /// </summary>
    public IConfigurationSection GetSection(string sectionKey)
    {
        var section = _configuration.GetSection(sectionKey);
        if (!section.Exists()) throw new Exception($"❌ Section '{sectionKey}' not found in appsettings.json!");
        return section;
    }
}