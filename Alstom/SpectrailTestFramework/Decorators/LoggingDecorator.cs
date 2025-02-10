using Alstom.Spectrail.Framework.Actions;

using Serilog;

using System.Threading.Tasks;
namespace Alstom.Spectrail.Framework.Decorators
{
    public class LoggingDecorator : BaseActionDecorator
    {
        public LoggingDecorator(IActionHandler wrappedAction) : base(wrappedAction) { }
        public override async Task HandleAsync()
        {
            Log.Information($"Executing: {_wrappedAction.GetType().Name}");
            await base.HandleAsync();
        }
    }
}