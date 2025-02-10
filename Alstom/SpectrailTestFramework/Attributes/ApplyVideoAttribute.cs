using Microsoft.Playwright;

using System;
using System.Threading.Tasks;
using Alstom.Spectrail.Framework.Actions;

namespace Alstom.Spectrail.Framework.Decorators
{
    public class ApplyVideoAttribute : DecoratorAttribute
    {
        public override IActionHandler Apply(IActionHandler action)
        {
            return new VideoDecorator(action);
        }
    }
}