namespace SpectrailTestDataProvider.API.Utility;

public class ServerConfigHelper
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
    public string? GetSetting(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("❌ Value cannot be null or empty.", nameof(key));

        // ✅ Check all configuration sources (Supports Playwright, ServerSettings, and General Settings)
        var value = _configuration[$"Settings:{key}"]
                    ?? _configuration[$"ServerSettings:{key}"]
                    ?? _configuration[$"PlaywrightSettings:{key}"];

        if (value == null)
            throw new Exception($"❌ Setting '{key}' not found in appsettings.json!");

        // ✅ If the setting is a file path, convert it to an absolute path
        return ResolvePath(value);
    }

    /// <summary>
    ///     ✅ Converts relative paths to absolute OS-compatible paths.
    /// </summary>
    private string ResolvePath(string path)
    {
        if (string.IsNullOrEmpty(path)) return path;

        // ✅ If already an absolute path, return it
        if (Path.IsPathRooted(path))
            return path;

        // ✅ Determine base path based on OS
        string basePath;
        if (OperatingSystem.IsWindows())
            basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SpectrailArtifacts");
        else
            basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "SpectrailArtifacts");

        return Path.Combine(basePath, path);
    }

    /// <summary>
    ///     ✅ Retrieves an integer setting (default = 0)
    /// </summary>
    public int GetIntSetting(string key, int defaultValue = 0)
    {
        var value = GetSetting(key);
        return int.TryParse(value, out var result) ? result : defaultValue;
    }

    /// <summary>
    ///     ✅ Retrieves a boolean setting (default = false)
    /// </summary>
    public bool GetBoolSetting(string key, bool defaultValue = false)
    {
        var value = GetSetting(key);
        return bool.TryParse(value, out var result) ? result : defaultValue;
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