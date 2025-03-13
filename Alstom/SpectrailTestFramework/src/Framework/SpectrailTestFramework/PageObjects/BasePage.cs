#region

using Microsoft.Playwright;
using SpectrailTestFramework.Interfaces;
using SpectrailTestFramework.Utilities;

#endregion

namespace SpectrailTestFramework.PageObjects;

/// <summary>
///     ✅ **Base class for all Playwright Page Objects.**
///     ✅ **Ensures each page object has access to Playwright's `IPage`.**
///     ✅ **Provides common navigation and wait methods.**
/// </summary>
public abstract class BasePage : IPageObject
{
    /// ✅ **Constructor now takes `IPage` directly (avoids circular DI issue)**
    protected BasePage(IPage page)
    {
        Page = page ?? throw new ArgumentNullException(nameof(page));
    }

    /// <summary>
    ///     ✅ **Predefined Property to Get ConfigHelper Easily**
    /// </summary>
    protected ConfigHelper Config => GetService<ConfigHelper>();

    public IPage Page { get; }

    /// <summary>
    ///     ✅ **Generic Property for Resolving Any Service from ServiceLocator**
    /// </summary>
    protected T GetService<T>() where T : notnull
    {
        return ServiceLocator.GetService<T>();
    }
}