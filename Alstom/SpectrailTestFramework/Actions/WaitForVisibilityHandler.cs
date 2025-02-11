using Microsoft.Playwright;

using SpectrailTestFramework.Factory;
using SpectrailTestFramework.PageObjects;

namespace SpectrailTestFramework.Actions;

public class WaitForVisibilityHandler : BaseActionHandler
{
    private readonly IPage _page;
    private readonly string _selector;

    public WaitForVisibilityHandler(ActionFactory actionFactory, string selector)
        : base(actionFactory)
    {
        _page = _actionFactory.CreatePage<BasePage>().Page; // ✅ Dynamically retrieves the Playwright page
        _selector = selector;
    }

    public override IPage? Page => _page; // ✅ Exposes Playwright Page for decorators

    protected override async Task ExecuteAsync()
    {
        await _page.WaitForSelectorAsync(_selector,
            new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
    }
}