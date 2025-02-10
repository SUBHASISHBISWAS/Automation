using Alstom.Spectrail.Framework.Actions;
using Microsoft.Playwright;



namespace Alstom.Spectrail.Framework.Decorators
{
    public class ApplyLoggingAttribute : DecoratorAttribute
    {
        public override IActionHandler Apply(IActionHandler action)
        {
            return new LoggingDecorator(action); // ✅ Ensure this returns IActionHandler
        }
    }
}