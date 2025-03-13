#region

using Alstom.Spectrail.TestFramework.Actions;
using Alstom.Spectrail.TestFramework.API.Services;
using Alstom.Spectrail.TestFramework.Attributes;
using Alstom.Spectrail.TestFramework.Factory;
using Alstom.Spectrail.TestFramework.Interfaces;
using Alstom.Spectrail.Tests.Pages;

#endregion

namespace Alstom.Spectrail.Tests.Handlers;

[MapsToPage(typeof(SpectrailNewFeatureHandlerPage))]
public class SpectrailNewFeatureHandler(
    IPageObject pageObject,
    IHandlerFactory handlerFactory,
    ApiServiceFactory apiServiceFactory) : BaseActionHandler
{
    private readonly ApiServiceFactory _apiServiceFactory =
        apiServiceFactory ?? throw new ArgumentNullException(nameof(apiServiceFactory));

    private readonly IHandlerFactory _handlerFactory =
        handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));

    private readonly SpectrailNewFeatureHandlerPage _spectrailNewFeaturePage =
        pageObject as SpectrailNewFeatureHandlerPage ??
        throw new ArgumentException(
            "Invalid PageObject type.");

    protected override async Task ExecuteAsync()
    {
        // ✅ Fetch Data from User Service
        var icdDataService = _apiServiceFactory.GetService("ICDDataService") as ICDDataService ??
                             throw new ArgumentNullException(nameof(ICDDataService))
            ;
        var userResponse = await icdDataService.GetICDDataAsync();
    }
}