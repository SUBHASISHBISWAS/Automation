using Microsoft.Playwright;

using SpectrailTestFramework.Interfaces;

namespace SpectrailTestFramework.PageObjects;

/// <summary>
/// ✅ **Base class for all Playwright Page Objects.**
/// ✅ **Ensures each page object has access to Playwright's `IPage`.**
/// ✅ **Provides common navigation and wait methods.**
/// </summary>
public abstract class BasePage : IPageObject
{
    public IPage Page { get; }

    /// ✅ **Constructor now takes `IPage` directly (avoids circular DI issue)**
    protected BasePage(IPage page)
    {
        Page = page ?? throw new ArgumentNullException(nameof(page));
    }
}