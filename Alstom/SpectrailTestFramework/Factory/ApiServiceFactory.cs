#region

using Microsoft.Extensions.DependencyInjection;
using SpectrailTestFramework.Services;

#endregion

namespace SpectrailTestFramework.Factory;

public class ApiServiceFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Type> _services;

    public ApiServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        // ✅ Dynamically register all classes implementing `IApiService`
        _services = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IApiService).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
            .ToDictionary(t => t.Name, t => t);
    }

    /// <summary>
    ///     ✅ Gets an API service dynamically based on name
    /// </summary>
    public IApiService GetService(string serviceName)
    {
        if (_services.TryGetValue(serviceName, out var serviceType))
            return (IApiService)_serviceProvider.GetRequiredService(serviceType);
        throw new InvalidOperationException($"API Service '{serviceName}' not found.");
    }
}