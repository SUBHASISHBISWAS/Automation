#region

using Alstom.Spectrail.TestFramework.Actions;
using Alstom.Spectrail.TestFramework.Attributes;
using Alstom.Spectrail.TestFramework.Factory;
using Alstom.Spectrail.TestFramework.Interfaces;
using Alstom.Spectrail.Tests.Pages;

#endregion

namespace Alstom.Spectrail.Tests.Handlers;

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