using Allure.Commons;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;

using NUnit.Allure.Attributes;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

using Serilog;

using SpectrailTestFramework.Factory;
using SpectrailTestFramework.Utilities;

using TestContext = NUnit.Framework.TestContext;

namespace SpectrailTests.Hooks;

[TestFixture]
[Parallelizable(ParallelScope.Fixtures)] // ✅ Use Fixtures instead of All to prevent shared state issues
[AllureSuite("Playwright Tests")]
public class TestHooks
{
    [OneTimeSetUp]
    public async Task Setup()
    {
        ServiceProvider = await PlaywrightFactory.SetupDependencies();
        Page = ServiceProvider.GetRequiredService<IPage>();
        ActionFactory = ServiceProvider.GetRequiredService<ActionFactory>();

        AllureLifecycle.Instance.CleanupResultDirectory();
        Log.Information("Test setup completed.");
    }

    [TearDown]
    public async Task TearDown()
    {
        string testName = TestContext.CurrentContext.Test.Name;
        TestStatus testStatus = TestContext.CurrentContext.Result.Outcome.Status;

        if (testStatus == TestStatus.Failed)
        {
            Log.Error($"Test Failed: {testName}");

            // ✅ Capture screenshot on failure
            string screenshotDir = Path.Combine(Directory.GetCurrentDirectory(), "screenshots");
            Directory.CreateDirectory(screenshotDir);
            string screenshotPath = Path.Combine(screenshotDir, $"{testName}.png");
            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });

            // ✅ Attach screenshot to Allure & Extent Reports
            //AllureLifecycle.Instance.AddAttachment("Failure Screenshot", "image/png", screenshotPath);
            //ExtentReportManager.AttachScreenshot(screenshotPath);

            // ✅ Capture video if enabled
            if (Page.Video != null)
            {
                string videoPath = await Page.Video.PathAsync();
                if (!string.IsNullOrEmpty(videoPath))
                {
                    string videoDir = Path.Combine(Directory.GetCurrentDirectory(), "Videos");
                    Directory.CreateDirectory(videoDir);
                    string savedVideoPath = Path.Combine(videoDir, $"{testName}.webm");
                    File.Move(videoPath, savedVideoPath);
                    Log.Information($"Failure video saved at: {savedVideoPath}");
                }
            }
        }
    }

    [OneTimeTearDown]
    public async Task Cleanup()
    {
        if (Page != null)
        {
            await Page.CloseAsync();
        }

        ServiceProvider?.Dispose();
    }

    public ServiceProvider? ServiceProvider { get; private set; }
    public IPage? Page { get; private set; }
    public ActionFactory? ActionFactory { get; private set; }
}