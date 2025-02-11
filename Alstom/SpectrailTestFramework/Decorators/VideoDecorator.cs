using Microsoft.Playwright;

using NUnit.Framework;
using NUnit.Framework.Interfaces;

using Serilog;

using SpectrailTestFramework.Interfaces;

namespace SpectrailTestFramework.Decorators;

public class VideoDecorator(IActionHandler wrappedAction) : BaseActionDecorator(wrappedAction)
{
    private readonly string _testName = TestContext.CurrentContext.Test.Name;

    private readonly string _videoDirectory =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpectrailArtifacts",
            "Videos");

    private IBrowserContext? _context;
    private IPage? _videoPage;
    private string? _videoPath;

    public override async Task HandleAsync()
    {
        await StartVideoRecordingAsync();
        await base.HandleAsync();
        if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
        {
            await SaveVideoAsync();
        }
        else
        {
            await DeleteVideoAsync();
        }
    }

    private async Task StartVideoRecordingAsync()
    {
        IPage? page = Page;
        if (page != null)
        {
            BrowserNewContextOptions options = new()
            {
                RecordVideoDir = _videoDirectory,
                RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 }
            };
            _context = await page.Context.Browser.NewContextAsync(options);
            _videoPage = await _context.NewPageAsync();
            _videoPath = await _videoPage.Video.PathAsync();
            Log.Information($"📹 Video recording started for {_testName}");
        }
    }

    private async Task SaveVideoAsync()
    {
        if (!string.IsNullOrEmpty(_videoPath) && File.Exists(_videoPath))
        {
            string savedVideoPath = Path.Combine(_videoDirectory, _testName, "video.webm");
            Directory.CreateDirectory(Path.GetDirectoryName(savedVideoPath)!);
            File.Move(_videoPath, savedVideoPath, true);
            Log.Error($"❌ Test failed: Video saved at {savedVideoPath}");
        }
    }

    private async Task DeleteVideoAsync()
    {
        if (!string.IsNullOrEmpty(_videoPath) && File.Exists(_videoPath))
        {
            File.Delete(_videoPath);
            Log.Information($"✅ Test passed: Video deleted for {_testName}");
        }
    }
}