using Microsoft.Playwright;

using SpectrailTestFramework.Interfaces;

namespace SpectrailTestFramework.Decorators;

public class ScreenshotDecorator(IActionHandler wrappedAction, string screenshotPath)
    : BaseActionDecorator(wrappedAction)
{
    private readonly string _screenshotPath = screenshotPath;

    public override IPage? Page => _wrappedAction.Page; // ✅ Forward Page property

    public override async Task HandleAsync()
    {
        await base.HandleAsync();

        // ✅ Now uses Page property instead of `_wrappedAction.Page`
        if (Page != null)
        {
            string screenshotFile = Path.Combine(_screenshotPath, $"{Guid.NewGuid()}.png");
            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotFile });
        }
    }
}