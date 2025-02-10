using System;
using System.IO;
using System.Threading.Tasks;

using Alstom.Spectrail.Framework.Actions;

using Microsoft.Playwright;

namespace Alstom.Spectrail.Framework.Decorators
{
    public class ScreenshotDecorator : BaseActionDecorator
    {
        private readonly string _screenshotPath;

        public ScreenshotDecorator(IActionHandler wrappedAction, string screenshotPath)
            : base(wrappedAction)
        {
            _screenshotPath = screenshotPath;
        }

        public override async Task HandleAsync()
        {
            await base.HandleAsync();

            // ✅ Now uses Page property instead of `_wrappedAction.Page`
            if (Page != null)
            {
                var screenshotFile = Path.Combine(_screenshotPath, $"{Guid.NewGuid()}.png");
                await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotFile });
            }
        }

        public override IPage? Page => _wrappedAction.Page; // ✅ Forward Page property
    }
}