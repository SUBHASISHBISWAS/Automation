#region

using Microsoft.Extensions.Configuration;

#endregion

namespace SpectrailTestDataProvider.Application.Utility;

public class ApplicationConfigHelper
{
    private readonly IConfigurationRoot _configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory()) // ✅ Cross-platform base directory
        .AddJsonFile("appsettings.json", false, true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
            true, true) // ✅ Support for Dev/Test/Prod
        .AddEnvironmentVariables() // ✅ Enables Docker & OS overrides
        .Build();

    // ✅ Cross-platform base directory
    // ✅ Support for Dev/Test/Prod
    // ✅ Enables Docker & OS overrides

    /// <summary>
    ///     ✅ Retrieves a setting from `appsettings.json` while resolving paths dynamically.
    /// </summary>
    private string? GetSetting(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("❌ Value cannot be null or empty.", nameof(key));

        // ✅ Check all configuration sources (Supports Playwright, ServerSettings, and General Settings)
        var value = _configuration[$"Settings:{key}"]
                    ?? _configuration[$"ServerSettings:{key}"]
                    ?? _configuration[$"PlaywrightSettings:{key}"];

        if (value == null)
            throw new Exception($"❌ Setting '{key}' not found in appsettings.json!");

        // ✅ Convert paths dynamically if necessary
        return ResolvePath(value);
    }

    /// <summary>
    ///     ✅ Converts relative paths to absolute OS-compatible paths.
    /// </summary>
    private static string ResolvePath(string path)
    {
        if (string.IsNullOrEmpty(path)) return path;

        // ✅ If already an absolute path, return it
        if (Path.IsPathRooted(path))
            return path;

        // ✅ Determine base path based on OS
        var basePath = Path.Combine(
            OperatingSystem.IsWindows()
                ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "SpectrailArtifacts");

        return Path.Combine(basePath, path);
    }

    /// <summary>
    ///     ✅ Retrieves a list of ICD files from `appsettings.json`
    /// </summary>
    public List<string> GetICDFiles()
    {
        var filesString = GetSetting("ICD_Files");

        if (string.IsNullOrEmpty(filesString))
            return [];

        // ✅ Split CSV string into a list (if stored as comma-separated values in appsettings)
        var files = filesString.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries)
            .Select(f => ResolvePath(f.Trim()))
            .ToList();

        return files;
    }

    /// <summary>
    ///     ✅ Retrieves an integer setting (default = 0)
    /// </summary>
    private int GetIntSetting(string key, int defaultValue = 0)
    {
        var value = GetSetting(key);
        return int.TryParse(value, out var result) ? result : defaultValue;
    }

    /// <summary>
    ///     ✅ Retrieves a boolean setting (default = false)
    /// </summary>
    private bool GetBoolSetting(string key, bool defaultValue = false)
    {
        var value = GetSetting(key);
        return bool.TryParse(value, out var result) ? result : defaultValue;
    }

    /// <summary>
    ///     ✅ Retrieves if a feature is enabled via feature flags
    /// </summary>
    public bool IsFeatureEnabled(string feature)
    {
        return GetBoolSetting($"FeatureFlags:{feature}");
    }

    /// <summary>
    ///     ✅ Retrieves database connection string from `appsettings.json`
    /// </summary>
    public string GetDatabaseConnectionString()
    {
        return GetSetting("DatabaseSettings:ConnectionString")
               ?? throw new Exception("❌ Database connection string not found!");
    }

    /// <summary>
    ///     ✅ Retrieves database name from `appsettings.json`
    /// </summary>
    private string GetDatabaseName()
    {
        return GetSetting("DatabaseSettings:DatabaseName")
               ?? throw new Exception("❌ Database name not found!");
    }

    /// <summary>
    ///     ✅ Retrieves Configuration Sections
    /// </summary>
    public IConfigurationSection GetSection(string sectionKey)
    {
        var section = _configuration.GetSection(sectionKey);
        if (!section.Exists())
            throw new Exception($"❌ Section '{sectionKey}' not found in appsettings.json!");
        return section;
    }
}