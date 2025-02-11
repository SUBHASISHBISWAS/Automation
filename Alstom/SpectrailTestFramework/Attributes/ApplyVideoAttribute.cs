using SpectrailTestFramework.Decorators;
using SpectrailTestFramework.Interfaces;

namespace SpectrailTestFramework.Attributes;

public class ApplyVideoAttribute : DecoratorAttribute
{
    public override IActionHandler Apply(IActionHandler action)
    {
        return new VideoDecorator(action);
    }
}