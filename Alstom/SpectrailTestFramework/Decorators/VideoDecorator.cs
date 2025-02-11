using Microsoft.Playwright;

using NUnit.Framework;
using NUnit.Framework.Interfaces;

using Serilog;

using SpectrailTestFramework.Interfaces;

using System;
using System.IO;
using System.Threading.Tasks;

namespace SpectrailTestFramework.Decorators;

public class VideoDecorator(IActionHandler wrappedAction) : BaseActionDecorator(wrappedAction)
{
    private readonly string _testName = TestContext.CurrentContext.Test.Name;
    private readonly string _videoDirectory =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpectrailArtifacts", "Videos");

    private IBrowserContext? _videoContext;
    private IPage? _videoPage;
    private string? _videoPath;

    public override async Task HandleAsync()
    {
        try
        {
            await StartVideoRecordingAsync();
            await base.HandleAsync(); // ✅ Execute the wrapped action
        }
        catch (Exception ex)
        {
            Log.Error($"❌ Test failed with error: {ex.Message}");
            await SaveVideoAsync(); // ✅ Save video on test failure
            throw;
        }
        finally
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                await SaveVideoAsync(); // ✅ Save video if test fails
            }
            else
            {
                await DeleteVideoAsync(); // ✅ Delete video if test passes
            }

            await _videoContext?.CloseAsync(); // ✅ Ensure Playwright context is properly closed
        }
    }

    private async Task StartVideoRecordingAsync()
    {
        IPage? page = WrappedAction.Page; // ✅ Use WrappedAction's Page
        if (page == null)
        {
            Log.Warning("⚠️ No Playwright Page found. Video recording skipped.");
            return;
        }

        BrowserNewContextOptions options = new()
        {
            RecordVideoDir = _videoDirectory,
            RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 }
        };

        _videoContext = await page.Context.Browser.NewContextAsync(options);
        _videoPage = await _videoContext.NewPageAsync();
        await _videoPage.GotoAsync(page.Url); // ✅ Sync video page with test execution
        _videoPath = await _videoPage.Video.PathAsync();

        Log.Information($"📹 Video recording started for {_testName}");
    }

    private async Task SaveVideoAsync()
    {
        if (string.IsNullOrEmpty(_videoPath) || !File.Exists(_videoPath))
        {
            Log.Warning($"⚠️ No video recorded for test {_testName}");
            return;
        }

        string savedVideoPath = Path.Combine(_videoDirectory, $"{_testName}.webm");
        Directory.CreateDirectory(Path.GetDirectoryName(savedVideoPath)!);

        File.Move(_videoPath, savedVideoPath, true);
        Log.Error($"❌ Test failed: Video saved at {savedVideoPath}");
    }

    private async Task DeleteVideoAsync()
    {
        if (string.IsNullOrEmpty(_videoPath) || !File.Exists(_videoPath))
        {
            return;
        }

        File.Delete(_videoPath);
        Log.Information($"✅ Test passed: Video deleted for {_testName}");
    }
}