using Microsoft.Playwright;

namespace SpectrailTestFramework.PageObjects;

public abstract class BasePage(IPage page)
{
    public readonly IPage Page = page;

    /// <summary>
    ///     Navigate to a URL
    /// </summary>
    public async Task GoToUrl(string url)
    {
        await Page.GotoAsync(url);
    }

    /// <summary>
    ///     Wait until the page has fully loaded
    /// </summary>
    public async Task WaitForPageLoad()
    {
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}