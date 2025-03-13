#region

using SpectrailTestFramework.Actions;
using SpectrailTestFramework.Attributes;
using SpectrailTestFramework.Factory;
using SpectrailTestFramework.Interfaces;
using SpectrailTests.Pages;

#endregion

namespace SpectrailTests.Handlers;

[MapsToPage(typeof(SpectrailHomePage))] // ✅ Automatically maps to `LoginPage`
public class SpectrailPageHandler(
    IPageObject pageObject,
    IHandlerFactory handlerFactory,
    ApiServiceFactory apiServiceFactory)
    : BaseActionHandler
{
    private readonly ApiServiceFactory _apiServiceFactory =
        apiServiceFactory ?? throw new ArgumentNullException(nameof(apiServiceFactory));

    private readonly IHandlerFactory _handlerFactory =
        handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));

    private readonly SpectrailHomePage _spectrailPage =
        pageObject as SpectrailHomePage ?? throw new ArgumentException("Invalid PageObject type.");


    protected override async Task ExecuteAsync()
    {
        await _spectrailPage.GoToAliviaProjectAndVerifyLink();
    }
}