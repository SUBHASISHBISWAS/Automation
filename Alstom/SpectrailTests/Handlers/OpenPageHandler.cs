using Microsoft.Playwright;

using SpectrailTestFramework.Attributes;
using SpectrailTestFramework.Interfaces;
using SpectrailTestFramework.PageObjects;
using SpectrailTests.Pages;

namespace SpectrailTestFramework.Actions
{
    [MapsToPage(typeof(OpenPage))] // ✅ Automatically maps to `LoginPage`
    public class OpenPageHandler : BaseActionHandler
    {
        private readonly IPage _page;
        private string? _url;

        public OpenPageHandler(IPageObject pageObject) // ✅ Inject the correct page
        {
            _page = pageObject.Page ?? throw new ArgumentNullException(nameof(pageObject));
        }

        public OpenPageHandler WithUrl(string url)
        {
            _url = url ?? throw new ArgumentNullException(nameof(url));
            return this;
        }

        protected override async Task ExecuteAsync()
        {
            if (string.IsNullOrEmpty(_url))
            {
                throw new InvalidOperationException("❌ URL must be set before executing OpenPageHandler.");
            }

            await _page.GotoAsync(_url);
        }
    }
}