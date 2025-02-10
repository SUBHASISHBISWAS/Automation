using Microsoft.Playwright;
using System;
using System.IO;
using System.Threading.Tasks;
using Alstom.Spectrail.Framework.Actions;

namespace Alstom.Spectrail.Framework.Decorators
{
    public class VideoDecorator : BaseActionDecorator
    {
        private IBrowserContext? _context;
        private string _videoPath = string.Empty;

        public VideoDecorator(IActionHandler wrappedAction) : base(wrappedAction) { }

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
            catch (Exception ex)
            {
                Console.WriteLine($"Test failed, video saved at: {_videoPath}");
                throw;
            }
        }

        private async Task EnableVideoRecordingAsync()
        {
            if (Page?.Context != null)
            {
                await Page.Context.CloseAsync(); // Close the existing context

                // ✅ Retrieve IBrowser from IPage.Context
                var browser = Page.Context.Browser;

                _context = await browser.NewContextAsync(new BrowserNewContextOptions
                {
                    RecordVideoDir = "videos/",
                    RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 }
                });

                await _context.NewPageAsync(); // ✅ Create a new page in the new context
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

        /// <summary>
        /// ✅ Properly forward Page from the wrapped action.
        /// </summary>
        public override IPage? Page => _wrappedAction.Page;
    }
}