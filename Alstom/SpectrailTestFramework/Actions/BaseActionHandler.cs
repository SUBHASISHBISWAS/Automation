using System.Diagnostics;

using Microsoft.Playwright;

using SpectrailTestFramework.Attributes;
using SpectrailTestFramework.Decorators;
using SpectrailTestFramework.Factory;
using SpectrailTestFramework.Interfaces;

namespace SpectrailTestFramework.Actions;

public abstract class BaseActionHandler(ActionFactory actionFactory) : IActionHandler
{
    protected readonly ActionFactory _actionFactory = actionFactory;
    private Func<Task>? _delayFunction;
    private IActionHandler? _nextHandler;

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

        await ExecuteAsync();
        if (_nextHandler != null)
        {
            await _nextHandler.HandleAsync();
        }
    }

    public async Task RunAsync()
    {
        await ApplyDecorators<BaseActionHandler>().HandleAsync();
    }

    public virtual IPage? Page => null;

    /// <summary>
    ///     Applies decorators dynamically while ensuring type consistency.
    /// </summary>
    public T ApplyDecorators<T>() where T : BaseActionHandler
    {
        IActionHandler decoratedAction = this; // Start with the original action

        IEnumerable<DecoratorAttribute> attributes = GetType()
            .GetCustomAttributes(typeof(DecoratorAttribute), true)
            .Cast<DecoratorAttribute>();

        foreach (DecoratorAttribute attribute in attributes)
        {
            decoratedAction = attribute.Apply(decoratedAction);
        }

        // ✅ Retrieve the original action from the decorator
        while (decoratedAction is BaseActionDecorator decorator)
        {
            decoratedAction = decorator.WrappedAction; // Unwrap the decorator
        }

        return decoratedAction is T typedAction
            ? typedAction
            : throw new InvalidOperationException(
                $"Decorator wrapping error: {decoratedAction.GetType().Name} cannot be cast to {typeof(T).Name}");
    }

    /// <summary>
    ///     Waits for a condition to be met within a timeout period.
    /// </summary>
    protected async Task WaitForConditionAsync(Func<Task<bool>> condition, int timeoutMilliseconds = 5000,
        int pollingInterval = 100)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
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
}