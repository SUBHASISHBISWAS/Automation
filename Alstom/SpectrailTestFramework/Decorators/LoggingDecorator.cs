using Serilog;

using SpectrailTestFramework.Decorators;
using SpectrailTestFramework.Interfaces;

public class LoggingDecorator : BaseActionDecorator
{
    public LoggingDecorator(IActionHandler wrappedAction) : base(wrappedAction)
    {
    }

    public override async Task HandleAsync()
    {
        Log.Information($"Starting action: {_wrappedAction.GetType().Name}");
        await base.HandleAsync();
        Log.Information($"Completed action: {_wrappedAction.GetType().Name}");
    }
}