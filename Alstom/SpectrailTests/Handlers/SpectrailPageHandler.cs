using Microsoft.Playwright;

using SpectrailTestFramework.Attributes;
using SpectrailTestFramework.Factory;
using SpectrailTestFramework.Interfaces;
using SpectrailTestFramework.PageObjects;
using SpectrailTests.Pages;

namespace SpectrailTestFramework.Actions
{
    [MapsToPage(typeof(SpectrailHomePage))] // ✅ Automatically maps to `LoginPage`
    public class SpectrailPageHandler : BaseActionHandler
    {

        private readonly IHandlerFactory _handlerFactory;
        private readonly SpectrailHomePage _spectrailPage;

        public SpectrailPageHandler(IPageObject pageObject, IHandlerFactory handlerFactory)
        {
            _spectrailPage = pageObject as SpectrailHomePage?? throw new ArgumentException("Invalid PageObject type.");
            _handlerFactory = handlerFactory;
        }


        protected override async Task ExecuteAsync()
        {
            await _spectrailPage.GoToAliviaProjectAndVerifyLink();
        }
    }
}