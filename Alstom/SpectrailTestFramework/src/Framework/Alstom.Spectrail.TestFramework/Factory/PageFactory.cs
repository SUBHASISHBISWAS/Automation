#region

using Alstom.Spectrail.TestFramework.Interfaces;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace Alstom.Spectrail.TestFramework.Factory;

public class PageFactory(IServiceProvider serviceProvider) : IPageFactory
{
    public T CreatePage<T>() where T : IPageObject
    {
        return serviceProvider.GetRequiredService<T>();
    }
}