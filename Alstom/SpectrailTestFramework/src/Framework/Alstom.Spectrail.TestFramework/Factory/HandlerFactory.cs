#region

using Alstom.Spectrail.TestFramework.Interfaces;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace Alstom.Spectrail.TestFramework.Factory;

public class HandlerFactory(IServiceProvider serviceProvider) : IHandlerFactory
{
    public T CreateHandler<T>() where T : IActionHandler
    {
        return serviceProvider.GetRequiredService<T>();
    }
}