using Microsoft.Playwright;

using SpectrailTestFramework.Interfaces;
using SpectrailTestFramework.PageObjects;

namespace SpectrailTests.Pages;

public class SpectrailHomePage(IPage page, IPageFactory pageFactory) : BasePage(page)
{
    private readonly IPageFactory _pageFactory = pageFactory;

    private ILocator _aliviaLabel => Page.GetByRole(AriaRole.Cell, new() { Name = "AVELIA project" });

    private ILocator _systemLabel => Page.GetByRole(AriaRole.Link, new() { Name = "System " });

    private ILocator _inputRequirementsLabel =>
        Page.GetByRole(AriaRole.Link, new() { Name = "Input Requirements" });

    public async Task GoToAliviaProjectAndVerifyLink()
    {
        await Page.RunAndWaitForNavigationAsync(async () => { await _aliviaLabel.ClickAsync(); },
            new PageRunAndWaitForNavigationOptions
            {
                Timeout = 30000000,
                UrlString = "https://spectrail-dev.alstom.hub/spectrailvalid/2/Home/Index"
            });

        await GoInputRequirementsAndVerifyLink();
    }

    public async Task GoInputRequirementsAndVerifyLink()
    {
        await _systemLabel.ClickAsync();
        await Task.Delay(2000);
        await Page.RunAndWaitForNavigationAsync(async () =>
            {
                await _inputRequirementsLabel.ClickAsync();
                await Task.Delay(2000);
            },
            new PageRunAndWaitForNavigationOptions
            {
                Timeout = 30000000,
                UrlString = "https://spectrail-dev.alstom.hub/spectrailvalid/2/Requirement/Requirements"
            });
    }


    public async Task<bool> IsProjectVisible()
    {
        return await Page.GetByRole(AriaRole.Heading, new() { Name = "Projects" }).IsVisibleAsync();
    }
}