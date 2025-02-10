using System.Threading.Tasks;

using Alstom.Spectrail.Framework.Actions;

using Microsoft.Playwright;

namespace Alstom.Spectrail.Framework.Decorators
{
    /// <summary>
    /// Base class for action decorators that extend functionalities dynamically.
    /// </summary>
    public abstract class BaseActionDecorator : IActionHandler
    {
        protected readonly IActionHandler _wrappedAction;

        protected BaseActionDecorator(IActionHandler wrappedAction)
        {
            _wrappedAction = wrappedAction;
        }

        /// <summary>
        /// Executes the decorated action.
        /// </summary>
        public virtual async Task HandleAsync()
        {
            await _wrappedAction.HandleAsync();
        }

        /// <summary>
        /// Supports chaining by passing the next handler to the wrapped action.
        /// </summary>
        public IActionHandler SetNext(IActionHandler nextHandler)
        {
            _wrappedAction.SetNext(nextHandler);
            return this;
        }

        /// <summary>
        /// Supports applying delays before execution.
        /// </summary>
        public IActionHandler WithDelay(Func<Task> delayFunction)
        {
            _wrappedAction.WithDelay(delayFunction);
            return this;
        }

        /// <summary>
        /// Exposes the Playwright Page if available from the wrapped action.
        /// </summary>
        public virtual IPage? Page => _wrappedAction.Page; // ✅ Forwards `Page` property dynamically
    }
}