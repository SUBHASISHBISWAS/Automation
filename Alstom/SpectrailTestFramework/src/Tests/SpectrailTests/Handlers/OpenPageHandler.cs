#region

using Microsoft.Playwright;
using Alstom.Spectrail.TestFramework.Actions;
using Alstom.Spectrail.TestFramework.Attributes;
using Alstom.Spectrail.TestFramework.Factory;
using Alstom.Spectrail.TestFramework.Interfaces;
using SpectrailTests.Pages;

#endregion

namespace SpectrailTests.Handlers;

[MapsToPage(typeof(OpenPage))] // ✅ Automatically maps to `LoginPage`
public class OpenPageHandler(IPageObject pageObject, ApiServiceFactory apiServiceFactory) : BaseActionHandler
{
    private readonly ApiServiceFactory _apiServiceFactory =
        apiServiceFactory ?? throw new ArgumentNullException(nameof(apiServiceFactory));

    private readonly IPage _page = pageObject.Page ?? throw new ArgumentNullException(nameof(pageObject));
    private string? _url;

    // ✅ Inject the correct page

    public OpenPageHandler WithUrl(string url)
    {
        _url = url ?? throw new ArgumentNullException(nameof(url));
        return this;
    }

    protected override async Task ExecuteAsync()
    {
        if (string.IsNullOrEmpty(_url))
            throw new InvalidOperationException("❌ URL must be set before executing OpenPageHandler.");

        await _page.GotoAsync(_url);
    }
}