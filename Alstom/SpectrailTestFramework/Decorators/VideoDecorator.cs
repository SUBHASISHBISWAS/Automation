using Microsoft.Playwright;

using SpectrailTestFramework.Interfaces;

namespace SpectrailTestFramework.Decorators;

public class VideoDecorator(IActionHandler wrappedAction) : BaseActionDecorator(wrappedAction)
{
    private IBrowserContext? _context;
    private string _videoPath = string.Empty;

    /// <summary>
    ///     ✅ Properly forward Page from the wrapped action.
    /// </summary>
    public override IPage? Page => _wrappedAction.Page;

    public override async Task HandleAsync()
    {
        try
        {
            // ✅ Ensure video recording is enabled before executing the action
            await EnableVideoRecordingAsync();

            await base.HandleAsync(); // ✅ Execute the wrapped action

            // ✅ Capture and save video path after execution
            _videoPath = await GetVideoPathAsync();
        }
        catch (Exception)
        {
            Console.WriteLine($"Test failed, video saved at: {_videoPath}");
            throw;
        }
    }

    private async Task EnableVideoRecordingAsync()
    {
        if (Page?.Context != null)
        {
            IBrowser? browser = Page.Context.Browser;
            _context = await browser.NewContextAsync(new BrowserNewContextOptions
            {
                RecordVideoDir = "videos/",
                RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 }
            });

            // ✅ Instead of creating a new page, set the context for the existing one.
            await _context.StorageStateAsync();
        }
    }

    private async Task<string> GetVideoPathAsync()
    {
        try
        {
            return await Page?.Video.PathAsync() ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}