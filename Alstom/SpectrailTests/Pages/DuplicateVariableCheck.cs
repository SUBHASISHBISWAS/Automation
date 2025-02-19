using Microsoft.Playwright;
using SpectrailTestFramework.Interfaces;
using SpectrailTestFramework.PageObjects;

namespace SpectrailTests.Pages;

public class DuplicateVariableCheck : BasePage
{
    private readonly IPageFactory _pageFactory;
    private IPage page;

    public DuplicateVariableCheck(IPage page, IPageFactory pageFactory) : base(page)
    {
        _pageFactory = pageFactory;
    }


    public async Task CheckDuplicate()
    {
        page = Page;
        await page.GotoAsync(Config.GetUrl("SpectrailValid"));
        await page.GetByRole(AriaRole.Cell, new() { Name = "AVELIA project" }).ClickAsync();
        await page.GetByRole(AriaRole.Link, new() { Name = "Architecture " }).ClickAsync();
        await page.GetByRole(AriaRole.Link, new() { Name = "Devices" }).ClickAsync();
        await page.GetByRole(AriaRole.Link, new() { Name = " Instantiations" }).ClickAsync();
        await page.Locator("#ListInstanciation_DXFREditorcol0").GetByRole(AriaRole.Cell).ClickAsync();
        await page.Locator("#ListInstanciation_DXFREditorcol0_I").FillAsync("Axlei");
        await page.Locator("#ListInstanciation_DXFREditorcol0_I").PressAsync("Enter");
        await page.GetByRole(AriaRole.Cell, new() { Name = "<Axlei>", Exact = true }).ClickAsync();
        await page.GetByRole(AriaRole.Textbox, new() { Name = "Enter substitution (one value" }).ClickAsync();
        await page.GetByRole(AriaRole.Textbox, new() { Name = "Enter substitution (one value" })
            .FillAsync("1\n2\nAxle1\n2");
        await page.GetByRole(AriaRole.Link, new() { Name = "Update" }).ClickAsync();
        await page.GetByText("Following are duplicate").ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Cancel" }).ClickAsync();
    }
}