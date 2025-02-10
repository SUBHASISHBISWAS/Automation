using Microsoft.Playwright;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Alstom.Spectrail.Framework.Decorators;
using Alstom.Spectrail.Framework.Utilities;

namespace Alstom.Spectrail.Framework.Actions
{
    public abstract class BaseActionHandler : IActionHandler
    {
        private IActionHandler? _nextHandler;
        private Func<Task>? _delayFunction = null;
        protected readonly ActionFactory _actionFactory;

        public BaseActionHandler(ActionFactory actionFactory)
        {
            _actionFactory = actionFactory;
        }

        public IActionHandler SetNext(IActionHandler nextHandler)
        {
            _nextHandler = nextHandler;
            return this;
        }

        public IActionHandler WithDelay(Func<Task> delayFunction)
        {
            _delayFunction = delayFunction;
            return this;
        }

        public async Task HandleAsync()
        {
            if (_delayFunction != null)
            {
                await _delayFunction.Invoke();
            }

            await ExecuteAsync(); // ✅ Calls the actual `ExecuteAsync()` method

            if (_nextHandler != null)
            {
                await _nextHandler.HandleAsync();
            }
        }

        /// <summary>
        /// Applies decorators dynamically using reflection.
        /// </summary>
        public IActionHandler ApplyDecorators()
        {
            IActionHandler action = this;

            var attributes = action.GetType()
                .GetCustomAttributes(typeof(DecoratorAttribute), true)
                .Cast<DecoratorAttribute>();

            foreach (var attribute in attributes)
            {
                action = attribute.Apply(action);
            }

            return action;
        }

        /// <summary>
        /// Waits for a condition to be met within a timeout period.
        /// </summary>
        protected async Task WaitForConditionAsync(Func<Task<bool>> condition, int timeoutMilliseconds = 5000, int pollingInterval = 100)
        {
            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds < timeoutMilliseconds)
            {
                if (await condition())
                {
                    return;
                }
                await Task.Delay(pollingInterval);
            }
            throw new TimeoutException("Condition not met within the specified timeout.");
        }

        /// <summary>
        /// New public method to execute the action, avoiding naming conflicts.
        /// </summary>
        public async Task RunAsync()
        {
            var decoratedAction = ApplyDecorators();
            await decoratedAction.HandleAsync();
        }

        /// <summary>
        /// Actual method each handler will implement.
        /// </summary>
        protected abstract Task ExecuteAsync();

        /// <summary>
        /// Exposes Playwright Page object if available. Otherwise, returns null.
        /// </summary>
        public virtual IPage? Page => null; // ✅ Default is null (for non-Playwright handlers)
    }
}