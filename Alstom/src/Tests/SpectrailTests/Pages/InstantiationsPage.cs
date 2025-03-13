#region

using Microsoft.Playwright;
using Serilog;
using SpectrailTestFramework.Interfaces;
using SpectrailTestFramework.PageObjects;

#endregion

namespace SpectrailTests.Pages;

public class InstantiationsPage(IPage page, IPageFactory pageFactory) : BasePage(page)
{
    public async Task OpenInstantiationAndEditVariable()
    {
        var spectrailUrl = Config.GetUrl("SpectrailValid");


        Log.Information("🚀 Test Started... Opening AVELIA project");
        await Page.GotoAsync(spectrailUrl, new PageGotoOptions
        {
            Timeout = 6000000
        });
        await Task.Delay(2000);

        Log.Information("🖱️ Clicking on AVELIA project...");
        await Page.GetByRole(AriaRole.Cell, new PageGetByRoleOptions { Name = "AVELIA project" }).ClickAsync();
        await Task.Delay(2000);

        Log.Information("🖱️ Clicking on 'Architecture'...");
        await Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Architecture " }).ClickAsync();
        await Task.Delay(2000);

        Log.Information("🖱️ Clicking on 'Devices'...");
        await Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Devices" }).ClickAsync();
        await Task.Delay(2000);

        Log.Information("🖱️ Clicking on 'Instantiations'...");
        await Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = " Instantiations" }).ClickAsync();
        await Task.Delay(2000);

        await Page.Locator("#ListInstanciation_DXFREditorcol0_I").ClickAsync();
        await Page.Locator("#ListInstanciation_DXFREditorcol0_I").FillAsync("Axlei");
        await Page.Locator("#ListInstanciation_DXFREditorcol0_I").PressAsync("Enter");
        await Task.Delay(2000);

        Log.Information("🖱️ Selecting 'Axlei'...");
        await Page.GetByRole(AriaRole.Cell, new PageGetByRoleOptions { Name = "<Axlei>", Exact = true }).ClickAsync();
        await Task.Delay(2000);

        Log.Information("🖱️ Clicking on the text box...");
        await Page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Enter substitution (one value" })
            .ClickAsync();
        await Task.Delay(2000);

        Log.Information("⌨️ Filling in substitution values...");
        await Page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Enter substitution (one value" })
            .FillAsync("Axle1\n1\n2\n2");
        await Task.Delay(2000);

        Log.Information("🖱️ Clicking on 'Update'...");
        await Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Update" }).ClickAsync();
        await Task.Delay(5000);

        Log.Information("🔍 Verifying the expected text...");
        try
        {
            Page.GetByText("Following are duplicate substitution(s) [2] !", new PageGetByTextOptions { Exact = true });
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