using Microsoft.Extensions.Configuration;

namespace SpectrailTestFramework.Utilities;

public class ConfigHelper
{
    private readonly IConfigurationRoot _configuration;

    public ConfigHelper()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                true, true) // ✅ Support for Dev/Test/Prod
            .AddEnvironmentVariables() // ✅ Enables environment variable overrides
            .Build();
    }

    /// <summary>
    ///     ✅ Retrieves a setting from `appsettings.json`
    /// </summary>
    public string GetSetting(string key)
    {
        return _configuration[$"PlaywrightSettings:{key}"] ??
               throw new Exception($"❌ Setting '{key}' not found in appsettings.json!");
    }

    /// <summary>
    ///     ✅ Retrieves a boolean setting (default = false)
    /// </summary>
    public bool GetBoolSetting(string key, bool defaultValue = false)
    {
        string? value = _configuration[$"PlaywrightSettings:{key}"];
        return bool.TryParse(value, out bool result) ? result : defaultValue;
    }

    /// <summary>
    ///     ✅ Retrieves an integer setting (default = 0)
    /// </summary>
    public int GetIntSetting(string key, int defaultValue = 0)
    {
        string? value = _configuration[$"PlaywrightSettings:{key}"];
        return int.TryParse(value, out int result) ? result : defaultValue;
    }

    /// <summary>
    ///     ✅ Retrieves URLs from `Settings` section
    /// </summary>
    public string GetUrl(string key)
    {
        return _configuration[$"Settings:{key}"] ??
               throw new Exception($"❌ URL '{key}' not found in appsettings.json!");
    }
}