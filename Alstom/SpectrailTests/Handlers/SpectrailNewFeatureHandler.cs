#region

using SpectrailTestFramework.Actions;
using SpectrailTestFramework.API.Services;
using SpectrailTestFramework.Attributes;
using SpectrailTestFramework.Factory;
using SpectrailTestFramework.Interfaces;
using SpectrailTests.Pages;

#endregion

namespace SpectrailTests.Handlers;

[MapsToPage(typeof(SpectrailNewFeatureHandlerPage))]
public class SpectrailNewFeatureHandler(
    IPageObject pageObject,
    IHandlerFactory handlerFactory,
    ApiServiceFactory apiServiceFactory) : BaseActionHandler
{
    private readonly ApiServiceFactory _apiServiceFactory =
        apiServiceFactory ?? throw new ArgumentNullException(nameof(apiServiceFactory));

    private readonly SpectrailNewFeatureHandlerPage _duplicateVariablePage =
        pageObject as SpectrailNewFeatureHandlerPage ??
        throw new ArgumentException(
            "Invalid PageObject type.");

    private readonly IHandlerFactory _handlerFactory =
        handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));

    protected override async Task ExecuteAsync()
    {
        // ✅ Fetch Data from User Service
        var icdDataService = _apiServiceFactory.GetService("ICDDataService") as ICDDataService ??
                             throw new ArgumentNullException(nameof(ICDDataService))
            ;
        var userResponse = await icdDataService.GetICDDataAsync();
    }
}