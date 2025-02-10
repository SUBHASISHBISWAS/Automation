using Alstom.Spectrail.Framework.Actions;
using Alstom.Spectrail.Framework.Decorators;
using Serilog;



public class LoggingDecorator : BaseActionDecorator
{
    public LoggingDecorator(IActionHandler wrappedAction) : base(wrappedAction) { }

    public override async Task HandleAsync()
    {
        Log.Information($"Starting action: {_wrappedAction.GetType().Name}");
        await base.HandleAsync();
        Log.Information($"Completed action: {_wrappedAction.GetType().Name}");
    }
}