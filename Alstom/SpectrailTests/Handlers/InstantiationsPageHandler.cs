using SpectrailTestFramework.Actions;
using SpectrailTestFramework.Attributes;
using SpectrailTestFramework.Interfaces;

using SpectrailTests.Pages;

namespace SpectrailTests.Handlers;

[MapsToPage(typeof(InstantiationsPage))] // ✅ Automatically maps to `LoginPage`
public class InstantiationsPageHandler(IPageObject pageObject, IHandlerFactory handlerFactory) : BaseActionHandler
{
    private readonly IHandlerFactory _handlerFactory = handlerFactory;

    private readonly InstantiationsPage _instantiationsPage = pageObject as InstantiationsPage ??
                                                              throw new ArgumentException("Invalid PageObject type.");

    protected override async Task ExecuteAsync()
    {
        await _instantiationsPage.OpenInstantiationAndEditVariable();
    }
}