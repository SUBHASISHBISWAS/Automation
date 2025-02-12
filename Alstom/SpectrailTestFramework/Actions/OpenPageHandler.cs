using System;
using System.Threading.Tasks;

using Microsoft.Playwright;

using SpectrailTestFramework.Decorators;
using SpectrailTestFramework.Factory;

namespace SpectrailTestFramework.Actions;

/// <summary>
/// ✅ **Action to open a page in Playwright.**
/// ✅ **Supports Fluent API (`WithUrl()`).**
/// ✅ **Uses middleware to apply decorators dynamically.**
/// </summary>
public class OpenPageHandler : BaseActionHandler
{
    private readonly IPage _page;
    private string? _url;

    public OpenPageHandler(ActionFactory actionFactory, IPage page) : base(actionFactory)
    {
        _page = page ?? throw new ArgumentNullException(nameof(page));
    }

    /// <summary>
    /// ✅ **Sets the URL dynamically using Fluent API.**
    /// </summary>
    public OpenPageHandler WithUrl(string url)
    {
        _url = url ?? throw new ArgumentNullException(nameof(url));
        return this;
    }

    /// <summary>
    /// ✅ **Executes the action (Navigates to the URL and waits for page load).**
    /// </summary>
    protected override async Task ExecuteAsync()
    {
        if (string.IsNullOrEmpty(_url))
        {
            throw new InvalidOperationException("❌ URL must be set before executing OpenPageHandler.");
        }

        await _page.GotoAsync(_url);
        await _page.WaitForLoadStateAsync(LoadState.Load); // ✅ Ensures the page is fully loaded
    }
}