using Microsoft.Extensions.DependencyInjection;

using SpectrailTestFramework.Interfaces;

namespace SpectrailTestFramework.Factory;

public class PageFactory(IServiceProvider serviceProvider) : IPageFactory
{
    public T CreatePage<T>() where T : IPageObject
    {
        return serviceProvider.GetRequiredService<T>();
    }
}