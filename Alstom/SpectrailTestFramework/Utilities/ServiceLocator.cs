using Microsoft.Extensions.DependencyInjection;

namespace SpectrailTestFramework.Utilities;

public static class ServiceLocator
{
    private static IServiceProvider? _serviceProvider;

    public static void Initialize(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public static T GetService<T>() where T : notnull
    {
        return _serviceProvider == null
            ? throw new InvalidOperationException(
                "❌ ServiceProvider is not initialized. Ensure PlaywrightFactory.SetupDependencies() is called.")
            : _serviceProvider.GetRequiredService<T>();
    }

    public static IServiceProvider GetProvider()
    {
        return _serviceProvider ?? throw new InvalidOperationException("❌ ServiceProvider is not initialized.");
    }
}