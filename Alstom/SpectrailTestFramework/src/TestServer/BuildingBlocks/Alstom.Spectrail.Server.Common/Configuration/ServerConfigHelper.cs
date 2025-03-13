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
    private readonly FeatureFlagsConfig _featureFlagsConfig;
    private readonly ICDConfig _icdConfig;

    /// <summary>
    ///     ✅ Supports Dependency Injection (Pass IConfiguration)
    /// </summary>
    public ServerConfigHelper(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        // ✅ Bind the "Settings" section to `ICDConfig`
        _icdConfig = configuration.GetSection("Settings").Get<ICDConfig>()
                     ?? throw new Exception("❌ Missing `Settings` section in appsettings.json!");

        // ✅ Bind the "FeatureFlags" section to `FeatureFlagsConfig`
        _featureFlagsConfig = configuration.GetSection("FeatureFlags").Get<FeatureFlagsConfig>()
                              ?? new FeatureFlagsConfig(); // Defaults to false if missing
    }

    /// <inheritdoc />
    public string GetICDFolderPath()
    {
        if (string.IsNullOrEmpty(_icdConfig.ICD_FOLDER_PATH))
            throw new Exception("❌ ICD_FOLDER_PATH is missing in appsettings.json!");

        var resolvedPath = ResolvePath(_icdConfig.ICD_FOLDER_PATH);
        if (!Directory.Exists(resolvedPath))
            throw new DirectoryNotFoundException($"❌ ICD folder not found: {resolvedPath}");

        return resolvedPath;
    }

    /// <inheritdoc />
    public List<string> GetICDFiles()
    {
        var icdFolderPath = GetICDFolderPath();
        var files = Directory.GetFiles(icdFolderPath, "*.xlsx").ToList();

        if (!files.Any())
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