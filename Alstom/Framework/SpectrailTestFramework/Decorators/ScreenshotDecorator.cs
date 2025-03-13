using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Serilog;
using SpectrailTestFramework.Interfaces;

namespace SpectrailTestFramework.Decorators;

public class ScreenshotDecorator : BaseActionDecorator
{
    private static readonly string ScreenshotDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpectrailArtifacts", "Screenshots");

    private readonly string _testName = TestContext.CurrentContext.Test.Name;

    public ScreenshotDecorator(IActionHandler wrappedAction) : base(wrappedAction)
    {
        string testScreenshotDirectory = Path.Combine(ScreenshotDirectory, _testName);
        Directory.CreateDirectory(testScreenshotDirectory);

        // ✅ Register middleware at the time of instantiation
        Use(Middleware());
    }

    public static Func<IActionHandler, Func<Task>, Task> Middleware()
    {
        return async (handler, next) =>
        {
            try
            {
                await next();
                var page = handler.Page;
                if (page != null && TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Inconclusive)
                {
                    string screenshotPath = Path.Combine(ScreenshotDirectory, TestContext.CurrentContext.Test.Name,
                        "failure.png");
                    await page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                    Log.Information($"📸 Screenshot saved: {screenshotPath}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"❌ ScreenshotDecorator encountered an error: {ex.Message}");
                throw;
            }
        };
    }
}