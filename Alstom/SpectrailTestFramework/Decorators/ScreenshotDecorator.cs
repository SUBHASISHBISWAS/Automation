using Microsoft.Playwright;

using NUnit.Framework;
using NUnit.Framework.Interfaces;

using Serilog;

using SpectrailTestFramework.Interfaces;

namespace SpectrailTestFramework.Decorators;

public class ScreenshotDecorator : BaseActionDecorator
{
    private readonly string _screenshotDirectory =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpectrailArtifacts",
            "Screenshots");

    private readonly string _testName = TestContext.CurrentContext.Test.Name;

    public ScreenshotDecorator(IActionHandler wrappedAction) : base(wrappedAction)
    {
        Directory.CreateDirectory(Path.Combine(_screenshotDirectory, _testName));
    }

    public override async Task HandleAsync()
    {
        try
        {
            await base.HandleAsync();
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                await CaptureScreenshotAsync();
            }
        }
        catch (Exception ex)
        {
            Log.Error($"❌ ScreenshotDecorator encountered an error: {ex.Message}");
            throw;
        }
    }

    private async Task CaptureScreenshotAsync()
    {
        IPage? page = Page;
        if (page != null)
        {
            string screenshotPath = Path.Combine(_screenshotDirectory, _testName, "failure.png");
            await page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
            Log.Information($"📸 Screenshot saved: {screenshotPath}");
        }
        else
        {
            Log.Warning("⚠️ Unable to capture screenshot: No Playwright page found.");
        }
    }
}