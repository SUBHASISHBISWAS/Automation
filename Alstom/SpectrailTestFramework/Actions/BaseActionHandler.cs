using System;
using System.Linq;
using System.Threading.Tasks;

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
        if (_nextHandler == null)
        {
            _nextHandler = nextHandler; // ✅ Ensure chaining is correct
        }
        else
        {
            _nextHandler.SetNext(nextHandler); // ✅ Chain properly if already set
        }
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

        await ExecuteAsync(); // ✅ Calls actual execution

        if (_nextHandler != null)
        {
            await _nextHandler.HandleAsync();
        }
    }

    public async Task RunAsync()
    {
        await HandleAsync(); // ✅ Directly executes the already decorated action
    }

    public virtual IPage? Page => null; // ✅ Default Page is null for non-Playwright handlers

    /// <summary>
    /// ✅ **Applies decorators dynamically while ensuring correct type safety.**
    /// </summary>
    public IActionHandler ApplyDecorators()
    {
        IActionHandler decoratedAction = this;

        var attributes = GetType()
            .GetCustomAttributes(typeof(DecoratorAttribute), true)
            .Cast<DecoratorAttribute>();

        foreach (var attribute in attributes)
        {
            decoratedAction = attribute.Apply(decoratedAction); // ✅ Apply each decorator
        }

        return decoratedAction; // ✅ Return wrapped action (ensuring type safety)
    }

    /// <summary>
    /// ✅ **Waits for a condition to be met within a timeout period.**
    /// </summary>
    protected async Task WaitForConditionAsync(Func<Task<bool>> condition, int timeoutMilliseconds = 5000,
        int pollingInterval = 100)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
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