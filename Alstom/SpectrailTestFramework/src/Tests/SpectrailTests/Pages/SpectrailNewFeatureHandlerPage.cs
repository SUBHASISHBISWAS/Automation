#region

using Microsoft.Playwright;
using Alstom.Spectrail.TestFramework.Interfaces;
using Alstom.Spectrail.TestFramework.PageObjects;

#endregion

namespace SpectrailTests.Pages;

public class SpectrailNewFeatureHandlerPage(IPage page, IPageFactory pageFactory) : BasePage(page)
{
    private readonly IPageFactory _pageFactory = pageFactory;
    private IPage _page = page;


    public async Task CheckDuplicate()
    {
        _page = Page;
        await _page.GotoAsync(Config.GetUrl("SpectrailValid"));
        await _page.GetByRole(AriaRole.Cell, new PageGetByRoleOptions { Name = "AVELIA project" }).ClickAsync();
        await _page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Architecture " }).ClickAsync();
        await _page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Devices" }).ClickAsync();
        await _page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = " Instantiations" }).ClickAsync();
        await _page.Locator("#ListInstanciation_DXFREditorcol0").GetByRole(AriaRole.Cell).ClickAsync();
        await _page.Locator("#ListInstanciation_DXFREditorcol0_I").FillAsync("Axlei");
        await _page.Locator("#ListInstanciation_DXFREditorcol0_I").PressAsync("Enter");
        await _page.GetByRole(AriaRole.Cell, new PageGetByRoleOptions { Name = "<Axlei>", Exact = true }).ClickAsync();
        await _page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Enter substitution (one value" })
            .ClickAsync();
        await _page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Enter substitution (one value" })
            .FillAsync("1\n2\nAxle1\n2");
        await _page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Update" }).ClickAsync();
        await _page.GetByText("Following are duplicate").ClickAsync();
        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Cancel" }).ClickAsync();
    }
}