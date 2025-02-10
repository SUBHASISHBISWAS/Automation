using System;

using Alstom.Spectrail.Framework.Actions;

namespace Alstom.Spectrail.Framework.Decorators
{
    /// <summary>
    /// Attribute that applies a ScreenshotDecorator automatically.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ApplyScreenshotAttribute : DecoratorAttribute
    {
        public override IActionHandler Apply(IActionHandler action)
        {
            return new ScreenshotDecorator(action, "screenshots/"); // ✅ Screenshot path can be configured
        }
    }
}