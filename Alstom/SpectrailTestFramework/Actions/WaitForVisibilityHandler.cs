using System;
using System.Threading.Tasks;

using Microsoft.Playwright;

using SpectrailTestFramework.Decorators;
using SpectrailTestFramework.Factory;
using SpectrailTestFramework.PageObjects;

namespace SpectrailTestFramework.Actions;

/// <summary>
/// ✅ **Action to wait for an element's visibility in Playwright.**
/// ✅ **Supports Fluent API (`WithSelector()`).**
/// ✅ **Uses middleware to apply decorators dynamically.**
/// </summary>
public class WaitForVisibilityHandler : BaseActionHandler
{
    private readonly IPage _page;
    private string _selector = string.Empty;

    public WaitForVisibilityHandler(ActionFactory actionFactory) : base(actionFactory)
    {
        _page = _actionFactory.CreatePage<BasePage>().Page ??
                throw new InvalidOperationException("❌ Page instance is null. Ensure Playwright is correctly initialized.");

       
    }

    public override IPage? Page => _page; // ✅ Exposes Playwright Page for decorators

    /// <summary>
    /// ✅ **Sets the selector dynamically using Fluent API.**
    /// </summary>
    public WaitForVisibilityHandler WithSelector(string selector)
    {
        _selector = selector ?? throw new ArgumentNullException(nameof(selector));
        return this;
    }

    /// <summary>
    /// ✅ **Executes the action (Waits for element visibility).**
    /// </summary>
    protected override async Task ExecuteAsync()
    {
        if (string.IsNullOrEmpty(_selector))
        {
            throw new InvalidOperationException("❌ Selector must be set before executing WaitForVisibilityHandler.");
        }

        await _page.WaitForSelectorAsync(_selector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
    }
}