using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Playwright;

namespace ExploringPlaywright.Pages
{
    public class SpectrailHomePageModel
    {
        private readonly IPage _page;

        public SpectrailHomePageModel(IPage page) => _page = page;
        private ILocator _aliviaLabel => _page.GetByRole(AriaRole.Cell, new() { Name = "AVELIA project" });

        private ILocator _systemLabel => _page.GetByRole(AriaRole.Link, new() { Name = "System " });

        private ILocator _inputRequirementsLabel =>
            _page.GetByRole(AriaRole.Link, new() { Name = "Input Requirements" });

        public async Task GoToAliviaProjectAndVerifyLink()
        {
           
            await _page.RunAndWaitForNavigationAsync(async () => { await _aliviaLabel.ClickAsync(); },
                new PageRunAndWaitForNavigationOptions()
                {
                    UrlString = "https://spectrail-dev.alstom.hub/spectrailvalid/2/Home/Index"
                });

        }

        public async Task GoInputRequirementsAndVerifyLink()
        {
            await _systemLabel.ClickAsync();
            await Task.Delay(2000);
            await _page.RunAndWaitForNavigationAsync(async () =>
                {
                    await _inputRequirementsLabel.ClickAsync();
                    await Task.Delay(2000);
                },
                new PageRunAndWaitForNavigationOptions()
                {
                    UrlString = "https://spectrail-dev.alstom.hub/spectrailvalid/2/Requirement/Requirements"
                });

        }

        

        public async Task<bool> IsProjectVisible() => await _page.GetByRole(AriaRole.Heading, new() { Name = "Projects" }).IsVisibleAsync();

        
    }
}
