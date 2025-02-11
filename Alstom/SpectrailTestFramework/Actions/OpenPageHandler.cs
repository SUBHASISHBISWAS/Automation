using Microsoft.Playwright;

using SpectrailTestFramework.Factory;

namespace SpectrailTestFramework.Actions;

public class OpenPageHandler(ActionFactory actionFactory, IPage page) : BaseActionHandler(actionFactory)
{
    private readonly IPage _page = page;
    private string? _url;


    public OpenPageHandler WithUrl(string url)
    {
        _url = url;
        return this; // ✅ Enables fluent chaining
    }

    protected override async Task ExecuteAsync()
    {
        if (string.IsNullOrEmpty(_url))
        {
            throw new InvalidOperationException("URL must be set before executing OpenPageHandler.");
        }

        await _page.GotoAsync(_url);
    }
}