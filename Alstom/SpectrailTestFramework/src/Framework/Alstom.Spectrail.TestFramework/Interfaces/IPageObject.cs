#region

using Microsoft.Playwright;

#endregion

namespace Alstom.Spectrail.TestFramework.Interfaces;

/// <summary>
///     ✅ **Marker interface for all Page Objects.**
///     ✅ **Ensures all pages have a reference to `IPage`.**
/// </summary>
public interface IPageObject
{
    IPage Page { get; }
}