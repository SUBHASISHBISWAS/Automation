using SpectrailTestFramework.Decorators;
using SpectrailTestFramework.Interfaces;

namespace SpectrailTestFramework.Attributes;

/// <summary>
///     Attribute that applies a ScreenshotDecorator automatically.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ApplyScreenshotAttribute : DecoratorAttribute
{
    public override IActionHandler Apply(IActionHandler action)
    {
        return new ScreenshotDecorator(action, "screenshots/"); // ✅ Screenshot path can be configured
    }
}