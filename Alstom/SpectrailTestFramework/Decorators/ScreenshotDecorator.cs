using Alstom.Spectrail.Framework.Actions;

using Microsoft.Playwright;

using System;
using System.Threading.Tasks;
namespace Alstom.Spectrail.Framework.Decorators
{
    public class ScreenshotDecorator : BaseActionDecorator
    {
        private readonly IPage _page;
        public ScreenshotDecorator(IActionHandler wrappedAction, IPage page) : base(wrappedAction)
        {
            _page = page;
        }
        public override async Task HandleAsync()
        {
            await base.HandleAsync();
            var screenshotPath = $"screenshots/{Guid.NewGuid()}.png";
            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
        }
    }
}