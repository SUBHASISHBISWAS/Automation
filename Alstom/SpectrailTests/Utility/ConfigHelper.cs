using Microsoft.Extensions.Configuration;

public class ConfigHelper
{
    private readonly IConfigurationRoot _configuration;

    public ConfigHelper()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    public string GetUrl(string key)
    {
        return _configuration[$"Settings:{key}"] ??
               throw new Exception($"❌ URL '{key}' not found in appsettings.json!");
    }
}