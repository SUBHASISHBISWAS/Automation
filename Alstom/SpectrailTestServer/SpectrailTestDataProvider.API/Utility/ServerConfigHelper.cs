#region

#endregion

namespace SpectrailTestDataProvider.API.Utility;

public class ServerConfigHelper
{
    private readonly IConfigurationRoot _configuration = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile("appsettings.json", false, true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
            true, true) // ✅ Support for Dev/Test/Prod
        .AddEnvironmentVariables() // ✅ Enables environment variable overrides
        .Build();

    // ✅ Support for Dev/Test/Prod
    // ✅ Enables environment variable overrides

    /// <summary>
    ///     ✅ Retrieves a setting from `appsettings.json`
    /// </summary>
    public string? GetSetting(string key)
    {
        if (string.IsNullOrEmpty(key)) throw new ArgumentException("Value cannot be null or empty.", nameof(key));
        if (_configuration[$"PlaywrightSettings:{key}"] != null) return _configuration[$"PlaywrightSettings:{key}"];

        if (_configuration[$"Settings:{key}"] != null) return _configuration[$"Settings:{key}"];

        if (_configuration[$"ServerSettings:{key}"] != null) return _configuration[$"ServerSettings:{key}"];

        throw new Exception($"❌ Setting '{key}' not found in appsettings.json!");
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
        return _configuration[$"Settings:{key}"] ??
               throw new Exception($"❌ URL '{key}' not found in appsettings.json!");
    }

    public IConfigurationSection GetSection(string sectionKey)
    {
        var section = _configuration.GetSection(sectionKey);
        if (!section.Exists()) throw new Exception($"❌ Section '{sectionKey}' not found in appsettings.json!");
        return section;
    }
}