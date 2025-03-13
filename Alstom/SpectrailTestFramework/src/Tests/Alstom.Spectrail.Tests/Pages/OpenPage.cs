#region

using Alstom.Spectrail.TestFramework.Interfaces;
using Alstom.Spectrail.TestFramework.PageObjects;
using Microsoft.Playwright;

#endregion

namespace Alstom.Spectrail.Tests.Pages;

/// <summary>
///     ✅ **Handles generic page navigation in Playwright.**
///     ✅ **Supports navigation & waiting for page load.**
/// </summary>
public class OpenPage(IPage page, IPageFactory pageFactory) : BasePage(page)
{
    private readonly IPageFactory _pageFactory = pageFactory;


    /// <summary>
    ///     ✅ **Navigate to a URL**
    /// </summary>
    public async Task GoToUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) throw new ArgumentException("❌ URL must not be empty.", nameof(url));

        await Page.GotoAsync(url, new PageGotoOptions
        {
            Timeout = 30000000
        });
    }


    /// <summary>
    ///     ✅ **Wait until the page has fully loaded**
    /// </summary>
    public async Task WaitForPageLoad()
    {
        await Page.WaitForLoadStateAsync(LoadState.Load);
    }
}