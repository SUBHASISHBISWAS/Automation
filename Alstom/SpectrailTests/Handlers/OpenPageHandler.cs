using Microsoft.Playwright;

using SpectrailTestFramework.Actions;
using SpectrailTestFramework.Attributes;
using SpectrailTestFramework.Interfaces;

using SpectrailTests.Pages;

namespace SpectrailTests.Handlers;

[MapsToPage(typeof(OpenPage))] // ✅ Automatically maps to `LoginPage`
public class OpenPageHandler(IPageObject pageObject) : BaseActionHandler
{
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
        {
            throw new InvalidOperationException("❌ URL must be set before executing OpenPageHandler.");
        }

        await _page.GotoAsync(_url);
    }
}