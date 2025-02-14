using System;
using System.Threading.Tasks;

using Microsoft.Playwright;

using SpectrailTestFramework.Interfaces;
using SpectrailTestFramework.PageObjects;

namespace SpectrailTests.Pages
{
    /// <summary>
    /// ✅ **Handles generic page navigation in Playwright.**
    /// ✅ **Supports navigation & waiting for page load.**
    /// </summary>
    public class OpenPage : BasePage
    {


        private readonly IPageFactory _pageFactory;
        public OpenPage(IPage page, IPageFactory pageFactory) : base(page)
        {
            _pageFactory = pageFactory;
        }
        

        /// <summary>
        /// ✅ **Navigate to a URL**
        /// </summary>
        public async Task GoToUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("❌ URL must not be empty.", nameof(url));
            }

            await Page.GotoAsync(url);
        }

        /// <summary>
        /// ✅ **Wait until the page has fully loaded**
        /// </summary>
        public async Task WaitForPageLoad()
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
    }
}