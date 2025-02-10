using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Alstom.Spectrail.Framework.PageObjects;
using Alstom.Spectrail.Framework.Utilities;
using NUnit.Framework;
using Serilog;
using System.IO;
using System.Threading.Tasks;
using NUnit.Allure.Attributes;
using Allure.Commons;
using NUnit.Framework.Interfaces;
using TestContext = NUnit.Framework.TestContext;

namespace Alstom.Spectrail.Tests.Hooks
{
    [TestFixture, Parallelizable(ParallelScope.Fixtures)] // ✅ Use Fixtures instead of All to prevent shared state issues
    [AllureSuite("Playwright Tests")]
    public class TestHooks
    {
        public ServiceProvider? ServiceProvider { get; private set; }
        public IPage? Page { get; private set; }
        public ActionFactory? ActionFactory { get; private set; }

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
            var testName = TestContext.CurrentContext.Test.Name;
            var testStatus = TestContext.CurrentContext.Result.Outcome.Status;

            if (testStatus == TestStatus.Failed)
            {
                Log.Error($"Test Failed: {testName}");

                // ✅ Capture screenshot on failure
                var screenshotDir = Path.Combine(Directory.GetCurrentDirectory(), "screenshots");
                Directory.CreateDirectory(screenshotDir);
                var screenshotPath = Path.Combine(screenshotDir, $"{testName}.png");
                await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });

                // ✅ Attach screenshot to Allure & Extent Reports
                //AllureLifecycle.Instance.AddAttachment("Failure Screenshot", "image/png", screenshotPath);
                //ExtentReportManager.AttachScreenshot(screenshotPath);

                // ✅ Capture video if enabled
                if (Page.Video != null)
                {
                    var videoPath = await Page.Video.PathAsync();
                    if (!string.IsNullOrEmpty(videoPath))
                    {
                        var videoDir = Path.Combine(Directory.GetCurrentDirectory(), "Videos");
                        Directory.CreateDirectory(videoDir);
                        var savedVideoPath = Path.Combine(videoDir, $"{testName}.webm");
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
            ServiceProvider.Dispose();
        }
    }
}