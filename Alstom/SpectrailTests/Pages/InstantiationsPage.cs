using Microsoft.Playwright;

using Serilog;

using SpectrailTestFramework.Interfaces;
using SpectrailTestFramework.PageObjects;

namespace SpectrailTests.Pages
{
    public class InstantiationsPage : BasePage
    {
        private readonly IPageFactory _pageFactory;

        public InstantiationsPage(IPage page, IPageFactory pageFactory) : base(page)
        {
            _pageFactory = pageFactory;
        }

        public async Task OpenInstantitionAndEditVariable()
        {
            string spectrailUrl = Config.GetUrl("SpectrailValid");


            Log.Information("🚀 Test Started... Opening AVELIA project");
            await Page.GotoAsync(spectrailUrl, new()
            {
                Timeout = 6000000
            });
            await Task.Delay(2000);

            Log.Information("🖱️ Clicking on AVELIA project...");
            await Page.GetByRole(AriaRole.Cell, new() { Name = "AVELIA project" }).ClickAsync();
            await Task.Delay(2000);

            Log.Information("🖱️ Clicking on 'Architecture'...");
            await Page.GetByRole(AriaRole.Link, new() { Name = "Architecture " }).ClickAsync();
            await Task.Delay(2000);

            Log.Information("🖱️ Clicking on 'Devices'...");
            await Page.GetByRole(AriaRole.Link, new() { Name = "Devices" }).ClickAsync();
            await Task.Delay(2000);

            Log.Information("🖱️ Clicking on 'Instantiations'...");
            await Page.GetByRole(AriaRole.Link, new() { Name = " Instantiations" }).ClickAsync();
            await Task.Delay(2000);

            await Page.Locator("#ListInstanciation_DXFREditorcol0_I").ClickAsync();
            await Page.Locator("#ListInstanciation_DXFREditorcol0_I").FillAsync("Axlei");
            await Page.Locator("#ListInstanciation_DXFREditorcol0_I").PressAsync("Enter");
            await Task.Delay(2000);

            Log.Information("🖱️ Selecting 'Axlei'...");
            await Page.GetByRole(AriaRole.Cell, new() { Name = "<Axlei>", Exact = true }).ClickAsync();
            await Task.Delay(2000);

            Log.Information("🖱️ Clicking on the text box...");
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter substitution (one value" }).ClickAsync();
            await Task.Delay(2000);

            Log.Information("⌨️ Filling in substitution values...");
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter substitution (one value" })
                .FillAsync("Axle1\n1\n2\n2");
            await Task.Delay(2000);

            Log.Information("🖱️ Clicking on 'Update'...");
            await Page.GetByRole(AriaRole.Link, new() { Name = "Update" }).ClickAsync();
            await Task.Delay(5000);

            Log.Information("🔍 Verifying the expected text...");
            try
            {
                Page.GetByText("Following are duplicate substitution(s) [2] !", new() { Exact = true });
                Console.WriteLine("✅ Text Verified: 'Following are duplicate substitution(s) [2] !' is present.");
                Log.Information("✅ Text Verified: 'Following are duplicate substitution(s) [2] !' is present.");
            }
            catch (Exception)
            {
                Log.Fatal("❌ Test Failed: Expected text not found!");
                Console.WriteLine("❌ Test Failed: Expected text not found!");
                Environment.Exit(1); // Fail the test
            }

            //await Task.Delay(2000);
            //await Page.GetByRole(AriaRole.Button, new() { Name = "Cancel" }).ClickAsync();
        }
    }
}