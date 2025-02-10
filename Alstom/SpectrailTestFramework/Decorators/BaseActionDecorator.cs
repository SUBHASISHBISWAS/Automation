using System;
using System.Threading.Tasks;

using Alstom.Spectrail.Framework.Actions;
namespace Alstom.Spectrail.Framework.Decorators
{
    public abstract class BaseActionDecorator : IActionHandler
    {
        protected readonly IActionHandler _wrappedAction;
        public Func<Task> DelayFunction { get; set; } = null;
        protected BaseActionDecorator(IActionHandler wrappedAction)
        {
            _wrappedAction = wrappedAction;
        }
        public virtual async Task HandleAsync()
        {
            if (DelayFunction != null)
            {
                await DelayFunction.Invoke();
            }
            await _wrappedAction.HandleAsync();
        }
        public void SetNext(IActionHandler nextHandler)
        {
            _wrappedAction.SetNext(nextHandler);
        }
    }
}