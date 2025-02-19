using SpectrailTestFramework.Actions;
using SpectrailTestFramework.Attributes;
using SpectrailTestFramework.Interfaces;
using SpectrailTests.Pages;

namespace SpectrailTests.Handlers;

[MapsToPage(typeof(SpectrailHomePage))] // ✅ Automatically maps to `LoginPage`
public class SpectrailPageHandler(IPageObject pageObject, IHandlerFactory handlerFactory) : BaseActionHandler
{
    private readonly IHandlerFactory _handlerFactory = handlerFactory;

    private readonly SpectrailHomePage _spectrailPage =
        pageObject as SpectrailHomePage ?? throw new ArgumentException("Invalid PageObject type.");


    protected override async Task ExecuteAsync()
    {
        await _spectrailPage.GoToAliviaProjectAndVerifyLink();
    }
}