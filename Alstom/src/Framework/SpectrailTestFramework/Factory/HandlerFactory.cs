#region

using Microsoft.Extensions.DependencyInjection;
using SpectrailTestFramework.Interfaces;

#endregion

namespace SpectrailTestFramework.Factory;

public class HandlerFactory(IServiceProvider serviceProvider) : IHandlerFactory
{
    public T CreateHandler<T>() where T : IActionHandler
    {
        return serviceProvider.GetRequiredService<T>();
    }
}