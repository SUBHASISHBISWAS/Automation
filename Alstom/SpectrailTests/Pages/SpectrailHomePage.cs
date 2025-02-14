using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Playwright;

using SpectrailTestFramework.Factory;
using SpectrailTestFramework.Interfaces;
using SpectrailTestFramework.PageObjects;

namespace SpectrailTests.Pages
{
    public class SpectrailHomePage: BasePage
    {
        

        private readonly IPageFactory _pageFactory;
        public SpectrailHomePage(IPage page, IPageFactory pageFactory) : base(page)
        {
            _pageFactory = pageFactory;
        }
        private ILocator _aliviaLabel => Page.GetByRole(AriaRole.Cell, new() { Name = "AVELIA project" });

        private ILocator _systemLabel => Page.GetByRole(AriaRole.Link, new() { Name = "System " });

        private ILocator _inputRequirementsLabel =>
            Page.GetByRole(AriaRole.Link, new() { Name = "Input Requirements" });

        public async Task GoToAliviaProjectAndVerifyLink()
        {
           
            await Page.RunAndWaitForNavigationAsync(async () => { await _aliviaLabel.ClickAsync(); },
                new PageRunAndWaitForNavigationOptions()
                {
                    UrlString = "https://spectrail-dev.alstom.hub/spectrailvalid/2/Home/Index"
                });

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
                new PageRunAndWaitForNavigationOptions()
                {
                    UrlString = "https://spectrail-dev.alstom.hub/spectrailvalid/2/Requirement/Requirements"
                });

        }

        

        public async Task<bool> IsProjectVisible() => await Page.GetByRole(AriaRole.Heading, new() { Name = "Projects" }).IsVisibleAsync();

        
    }
}
