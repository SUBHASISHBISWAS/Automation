#region

using Microsoft.Extensions.Configuration;

#endregion

namespace SpectrailTestDataProvider.Shared.Configuration;

/// <summary>
///     ✅ ServerConfigHelper dynamically reads configuration settings
///     from appsettings.json, environment variables, and Docker overrides.
/// </summary>
public class ServerConfigHelper
{
    private readonly IConfiguration _configuration;

    /// <summary>
    ///     ✅ Supports Dependency Injection (Pass IConfiguration)
    /// </summary>
    public ServerConfigHelper(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    ///     ✅ Retrieves a setting from `appsettings.json` dynamically
    /// </summary>
    public string? GetSetting(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("❌ Key cannot be null or empty.", nameof(key));

        var value = _configuration[$"Settings:{key}"]
                    ?? _configuration[$"ServerSettings:{key}"]
                    ?? _configuration[$"PlaywrightSettings:{key}"];

        if (value == null)
            throw new Exception($"❌ Setting '{key}' not found in configuration!");

        return ResolvePath(value); // ✅ Convert relative paths
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

    /// <summary>
    ///     ✅ Retrieves an integer setting from configuration (Default = 0)
    /// </summary>
    public int GetIntSetting(string key, int defaultValue = 0)
    {
        var value = _configuration[$"Settings:{key}"];
        return int.TryParse(value, out var result) ? result : defaultValue;
    }

    /// <summary>
    ///     ✅ Retrieves a boolean setting from configuration (Default = false)
    /// </summary>
    public bool GetBoolSetting(string key, bool defaultValue = false)
    {
        var value = _configuration[$"Settings:{key}"];
        return bool.TryParse(value, out var result) ? result : defaultValue;
    }

    /// <summary>
    ///     ✅ Retrieves Configuration Sections from `appsettings.json`
    /// </summary>
    public IConfigurationSection GetSection(string sectionKey)
    {
        var section = _configuration.GetSection(sectionKey);
        if (!section.Exists())
            throw new Exception($"❌ Section '{sectionKey}' not found in appsettings.json!");
        return section;
    }

    /// <summary>
    ///     ✅ Retrieves a list of ICD files from `appsettings.json`
    /// </summary>
    public List<string> GetICDFiles()
    {
        var filesString = GetSetting("ICD_Files");

        if (string.IsNullOrEmpty(filesString))
            return [];

        // ✅ Split CSV string into a list (Supports comma `,` or semicolon `;`)
        return filesString.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries)
            .Select(f => ResolvePath(f.Trim()))
            .ToList();
    }

    /// <summary>
    ///     ✅ Checks if a feature flag is enabled (Default = false)
    /// </summary>
    public bool IsFeatureEnabled(string feature)
    {
        var value = GetSetting($"FeatureFlags:{feature}");

        return bool.TryParse(value, out var result) && result;
    }
}