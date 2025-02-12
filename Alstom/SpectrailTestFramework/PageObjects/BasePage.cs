using System;
using System.Threading.Tasks;

using Microsoft.Playwright;

namespace SpectrailTestFramework.PageObjects;

/// <summary>
/// ✅ **Base class for Playwright pages.**
/// ✅ **Encapsulates common navigation and page interactions.**
/// ✅ **Ensures elements are loaded before interactions.**
/// </summary>
public abstract class BasePage
{
    public readonly IPage Page;

    public BasePage(IPage page)
    {
        Page = page ?? throw new ArgumentNullException(nameof(page));
    }

    /// <summary>
    /// ✅ **Navigates to a URL using Fluent API.**
    /// </summary>
    public async Task<BasePage> GoToUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentException("❌ URL must not be null or empty.", nameof(url));
        }

        await Page.GotoAsync(url);
        return this;
    }

    /// <summary>
    /// ✅ **Waits until the page has fully loaded.**
    /// </summary>
    public async Task<BasePage> WaitForPageLoad()
    {
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        return this;
    }

    /// <summary>
    /// ✅ **Waits for a specific selector to become visible.**
    /// </summary>
    public async Task<BasePage> WaitForElement(string selector, int timeoutMilliseconds = 5000)
    {
        if (string.IsNullOrEmpty(selector))
        {
            throw new ArgumentException("❌ Selector must not be null or empty.", nameof(selector));
        }

        await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = timeoutMilliseconds
        });

        return this;
    }
}