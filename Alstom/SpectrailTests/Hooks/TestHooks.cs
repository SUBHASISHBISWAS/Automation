using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Alstom.Spectrail.Framework.PageObjects;
using Alstom.Spectrail.Framework.Utilities;
using NUnit.Framework;
using Serilog;
using TestContext = NUnit.Framework.TestContext;
using NUnit.Allure.Attributes;
using Allure.Commons;
namespace Alstom.Spectrail.Tests.Hooks
{
    [TestFixture, Parallelizable(ParallelScope.All)]
    [AllureSuite("Playwright Tests")]
    public class TestHooks
    {
        public ServiceProvider ServiceProvider { get; private set; }
        public IPage Page { get; private set; }
        public LoginPage LoginPage { get; private set; }
        [OneTimeSetUp]
        public async Task Setup()
        {
            ServiceProvider = await PlaywrightFactory.SetupDependencies();
            Page = ServiceProvider.GetRequiredService<IPage>();
            LoginPage = ServiceProvider.GetRequiredService<LoginPage>();
            AllureLifecycle.Instance.CleanupResultDirectory();
            Log.Information("Test setup completed.");
        }

        [TearDown]
        public async Task TearDown()
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                Log.Error($"Test Failed: {TestContext.CurrentContext.Test.Name}");
                // Capture video path if test failed
                var videoPath = await Page.Video.PathAsync();
                var savedVideoPath = $"Videos/{TestContext.CurrentContext.Test.Name}.webm";
                System.IO.File.Move(videoPath, savedVideoPath);
                Log.Information($"Failure video saved at: {savedVideoPath}");

                var screenshotPath = $"screenshots/{TestContext.CurrentContext.Test.Name}.png";
                await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                // Attach screenshot to Allure report
                AllureLifecycle.Instance.AddAttachment("Failure Screenshot", "image/png", screenshotPath);
                ExtentReportManager.AttachScreenshot(screenshotPath);
            }
        }

        [OneTimeTearDown]
        public async Task Cleanup()
        {
            await Page.CloseAsync();
            ServiceProvider.Dispose();
        }
    }
}