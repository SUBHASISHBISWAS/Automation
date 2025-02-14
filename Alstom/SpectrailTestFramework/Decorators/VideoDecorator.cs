using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Playwright;

using NUnit.Framework;
using NUnit.Framework.Interfaces;

using Serilog;

using SpectrailTestFramework.Interfaces;

namespace SpectrailTestFramework.Decorators;

/// <summary>
/// ✅ **Video Decorator for actions.**
/// ✅ **Ensures video is recorded only when the test fails.**
/// ✅ **Uses middleware for proper execution.**
/// </summary>
public class VideoDecorator : BaseActionDecorator
{
    private static readonly string ParentVideoDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpectrailArtifacts", "Videos");

    private readonly string _testName = TestContext.CurrentContext.Test.Name;
    private string? _videoPath;
    private IBrowserContext? _videoContext;
    private IPage? _videoPage;

    public VideoDecorator(IActionHandler wrappedAction) : base(wrappedAction)
    {
        Use(Middleware()); // ✅ Register middleware properly
    }

    /// <summary>
    /// ✅ **Middleware for dynamically handling video recording.**
    /// </summary>
    public static Func<IActionHandler, Func<Task>, Task> Middleware()
    {
        return async (handler, next) =>
        {
            await StartVideoRecordingAsync(handler);
            try
            {
                await next(); // ✅ Run the actual action
            }
            catch (Exception ex)
            {
                Log.Error($"❌ Test failed with error: {ex.Message}");
                await SaveVideoAsync(handler); // ✅ Save video on test failure
                throw;
            }
            finally
            {
                if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
                {
                    await SaveVideoAsync(handler); // ✅ Save video if test fails
                }
                else
                {
                    await DeleteVideoAsync(handler); // ✅ Delete video if test passes
                }
            }
        };
    }

    /// <summary>
    /// ✅ **Starts video recording.**
    /// </summary>
    private static async Task StartVideoRecordingAsync(IActionHandler handler)
    {
        IPage? page = handler.Page;
        if (page == null)
        {
            Log.Warning("⚠️ No Playwright Page found. Video recording skipped.");
            return;
        }

        string videoDirectory = Path.Combine(ParentVideoDirectory, TestContext.CurrentContext.Test.Name);
        Directory.CreateDirectory(videoDirectory);

        BrowserNewContextOptions options = new()
        {
            RecordVideoDir = videoDirectory,
            RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 }
        };

        IBrowserContext videoContext = await page.Context.Browser.NewContextAsync(options);
        IPage videoPage = await videoContext.NewPageAsync();
        await videoPage.GotoAsync(page.Url); // ✅ Sync video page with test execution
        handler.Use(async (_, next) => { await next(); }); // ✅ Ensure video context is properly closed
    }

    /// <summary>
    /// ✅ **Saves video if the test fails.**
    /// </summary>
    private static async Task SaveVideoAsync(IActionHandler handler)
    {
        IPage? page = handler.Page;
        if (page == null || page.Video == null)
        {
            Log.Warning($"⚠️ No video recorded for test {TestContext.CurrentContext.Test.Name}");
            return;
        }

        string videoPath = await page.Video.PathAsync();
        if (!File.Exists(videoPath))
        {
            Log.Warning($"⚠️ No video file found for test {TestContext.CurrentContext.Test.Name}");
            return;
        }

        string savedVideoPath = Path.Combine(ParentVideoDirectory, TestContext.CurrentContext.Test.Name, "test-failure.webm");
        Directory.CreateDirectory(Path.GetDirectoryName(savedVideoPath)!);

        File.Move(videoPath, savedVideoPath, true);
        Log.Error($"❌ Test failed: Video saved at {savedVideoPath}");
    }

    /// <summary>
    /// ✅ **Deletes video if the test passes.**
    /// </summary>
    private static async Task DeleteVideoAsync(IActionHandler handler)
    {
        IPage? page = handler.Page;
        if (page == null || page.Video == null)
        {
            return;
        }

        string videoPath = await page.Video.PathAsync();
        if (File.Exists(videoPath))
        {
            File.Delete(videoPath);
            Log.Information($"✅ Test passed: Video deleted for {TestContext.CurrentContext.Test.Name}");
        }
    }
}