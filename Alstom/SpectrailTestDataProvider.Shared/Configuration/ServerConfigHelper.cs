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
    private readonly ICDConfig _icdConfig;

    /// <summary>
    ///     ✅ Supports Dependency Injection (Pass IConfiguration)
    /// </summary>
    public ServerConfigHelper(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _icdConfig = new ICDConfig();
        GetSection("Settings").Bind(_icdConfig);
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
    ///     ✅ Retrieves Configuration Sections from `appsettings.json`
    /// </summary>
    private IConfigurationSection GetSection(string sectionKey)
    {
        var section = _configuration.GetSection(sectionKey);
        if (!section.Exists())
            throw new Exception($"❌ Section '{sectionKey}' not found in appsettings.json!");
        return section;
    }

    /// <summary>
    ///     ✅ Retrieves a list of ICD files from `appsettings.json`
    /// </summary>
    public List<string>? GetICDFiles()
    {
        return _icdConfig.ICD_Files.Select(ResolvePath).ToList();
    }

    /// <summary>
    ///     ✅ Checks if a feature flag is enabled (Default = false)
    /// </summary>
    public bool IsFeatureEnabled(string feature)
    {
        var value = _configuration[$"FeatureFlags:{feature}"];
        return bool.TryParse(value, out var result) && result;
    }
}