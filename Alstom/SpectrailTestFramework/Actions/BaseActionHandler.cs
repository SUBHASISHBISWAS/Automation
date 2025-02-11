using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Alstom.Spectrail.Framework.Decorators;
using Alstom.Spectrail.Framework.Utilities;

using Microsoft.Playwright;

namespace Alstom.Spectrail.Framework.Actions
{
    public abstract class BaseActionHandler(ActionFactory actionFactory) : IActionHandler
    {
        private IActionHandler? _nextHandler;
        private Func<Task>? _delayFunction = null;
        protected readonly ActionFactory _actionFactory = actionFactory;

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
            if (_delayFunction != null) await _delayFunction.Invoke();
            await ExecuteAsync();
            if (_nextHandler != null) await _nextHandler.HandleAsync();
        }

        /// <summary>
        /// Applies decorators dynamically while ensuring type consistency.
        /// </summary>
        public T ApplyDecorators<T>() where T : BaseActionHandler
        {
            IActionHandler decoratedAction = this; // Start with the original action

            var attributes = GetType()
                .GetCustomAttributes(typeof(DecoratorAttribute), true)
                .Cast<DecoratorAttribute>();

            foreach (var attribute in attributes)
            {
                decoratedAction = attribute.Apply(decoratedAction);
            }

            // ✅ Retrieve the original action from the decorator
            while (decoratedAction is BaseActionDecorator decorator)
            {
                decoratedAction = decorator.WrappedAction; // Unwrap the decorator
            }

            if (decoratedAction is T typedAction)
            {
                return typedAction;
            }

            throw new InvalidOperationException($"Decorator wrapping error: {decoratedAction.GetType().Name} cannot be cast to {typeof(T).Name}");
        }

        public async Task RunAsync()
        {
            await ApplyDecorators<BaseActionHandler>().HandleAsync();
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

        protected abstract Task ExecuteAsync();

        public virtual IPage? Page => null;
    }
}