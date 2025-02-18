using SpectrailTestFramework.Actions;
using SpectrailTestFramework.Attributes;
using SpectrailTestFramework.Interfaces;
using SpectrailTests.Pages;

namespace SpectrailTests.Handlers
{
    [MapsToPage(typeof(InstantiationsPage))] // ✅ Automatically maps to `LoginPage`
    public class InstantiationsPageHandler : BaseActionHandler
    {
        private readonly IHandlerFactory _handlerFactory;
        private readonly InstantiationsPage _instantiationsPage;

        public InstantiationsPageHandler(IPageObject pageObject, IHandlerFactory handlerFactory)
        {
            _instantiationsPage = pageObject as InstantiationsPage ??
                                  throw new ArgumentException("Invalid PageObject type.");
            _handlerFactory = handlerFactory;
        }


        protected override async Task ExecuteAsync()
        {
            await _instantiationsPage.OpenInstantitionAndEditVariable();
        }
    }
}