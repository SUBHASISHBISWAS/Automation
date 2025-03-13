#region

using SpectrailTestFramework.Actions;
using SpectrailTestFramework.Attributes;
using SpectrailTestFramework.Factory;
using SpectrailTestFramework.Interfaces;
using SpectrailTests.Pages;

#endregion

namespace SpectrailTests.Handlers;

[MapsToPage(typeof(InstantiationsPage))] // ✅ Automatically maps to `InstantiationsPage`
public class InstantiationsPageHandler(
    IPageObject pageObject,
    IHandlerFactory handlerFactory,
    ApiServiceFactory apiServiceFactory)
    : BaseActionHandler
{
    private readonly ApiServiceFactory _apiServiceFactory =
        apiServiceFactory ?? throw new ArgumentNullException(nameof(apiServiceFactory));


    private readonly IHandlerFactory _handlerFactory =
        handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));

    private readonly InstantiationsPage _instantiationsPage = pageObject as InstantiationsPage ??
                                                              throw new ArgumentException("Invalid PageObject type.");

    protected override async Task ExecuteAsync()
    {
        await _instantiationsPage.OpenInstantiationAndEditVariable();
    }
}