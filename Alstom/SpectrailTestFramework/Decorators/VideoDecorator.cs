using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using Serilog;
using Alstom.Spectrail.Framework.Actions;
using Alstom.Spectrail.Framework.Decorators;
public class VideoDecorator : BaseActionDecorator
{
    private readonly IPage _page;
    public VideoDecorator(IActionHandler wrappedAction, IPage page) : base(wrappedAction)
    {
        _page = page;
    }
    public override async Task HandleAsync()
    {
        try
        {
            await _wrappedAction.HandleAsync();  // Execute the main action
        }
        catch (Exception ex)
        {
            Log.Error($"Test failed: {ex.Message}");
            // Save the video path if test fails
            var videoPath = await _page.Video.PathAsync();
            var savedVideoPath = $"Videos/{Guid.NewGuid()}.webm";
            System.IO.File.Move(videoPath, savedVideoPath);
            Log.Information($"Test failure video saved at: {savedVideoPath}");
            throw;  // Rethrow exception to ensure test failure is reported
        }
    }
}