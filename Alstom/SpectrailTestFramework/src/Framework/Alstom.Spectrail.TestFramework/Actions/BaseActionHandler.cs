#region

using System.Diagnostics;
using Alstom.Spectrail.TestFramework.Interfaces;
using Alstom.Spectrail.TestFramework.Utilities;
using Microsoft.Playwright;
using Serilog;

#endregion

namespace Alstom.Spectrail.TestFramework.Actions;

/// <summary>
///     ✅ **Base class for all actions, supporting middleware-based execution.**
///     ✅ **Ensures decorators execute dynamically before the actual action.**
///     ✅ **Supports Fluent API for chaining actions and decorators.**
/// </summary>
public abstract class BaseActionHandler : IActionHandler
{
    private readonly List<Func<IActionHandler, Func<Task>, Task>> _middlewares = new(); // ✅ Middleware pipeline
    private Func<Task>? _delayFunction;
    private IActionHandler? _nextHandler;

    /// <summary>
    ///     ✅ **Predefined Property to Get ConfigHelper Easily**
    /// </summary>
    protected ConfigHelper Config => GetService<ConfigHelper>();

    public IActionHandler? DecoratedInstance { get; set; } // ✅ Exposes decorated instance as a property

    /// <summary>
    ///     ✅ **Supports Fluent API for chaining actions.**
    /// </summary>
    public IActionHandler SetNextAction(IActionHandler nextHandler)
    {
        if (_nextHandler == null)
        {
            _nextHandler = nextHandler;
            Log.Information($"🔗 Chained {GetType().Name} ➡ {nextHandler.GetType().Name}");
        }
        else
        {
            _nextHandler.SetNextAction(nextHandler);
        }

        return this;
    }

    /// <summary>
    ///     ✅ **Allows adding an optional delay before execution.**
    /// </summary>
    public IActionHandler WithDelay(Func<Task> delayFunction)
    {
        _delayFunction = delayFunction;
        return this;
    }

    /// <summary>
    ///     ✅ **Registers middleware dynamically before execution.**
    /// </summary>
    public IActionHandler Use(Func<IActionHandler, Func<Task>, Task> middleware)
    {
        _middlewares.Add(middleware);
        return this;
    }

    /// <summary>
    ///     ✅ **Handles the action, ensuring middleware and chaining work correctly.**
    /// </summary>
    public async Task HandleAsync()
    {
        var index = -1;

        async Task Next()
        {
            index++;

            if (index < _middlewares.Count)
            {
                Log.Information($"🔹 Running Middleware [{index}] for {GetType().Name}");
                await _middlewares[index](this, Next); // ✅ Middleware executes first
            }
            else
            {
                if (_delayFunction != null)
                {
                    Log.Information($"⏳ Applying delay before execution in {GetType().Name}");
                    await _delayFunction();
                }

                Log.Information($"🚀 Executing action: {GetType().Name}");
                await ExecuteAsync(); // ✅ Calls the actual action

                if (_nextHandler != null)
                {
                    Log.Information($"➡ Moving to next action: {_nextHandler.GetType().Name}");
                    await _nextHandler.HandleAsync(); // ✅ Calls the next action in the chain
                }
            }
        }

        Log.Information($"🔄 Starting execution pipeline for {GetType().Name}");
        await Next(); // ✅ Start the middleware execution pipeline
    }

    /// <summary>
    ///     ✅ **Executes the fully decorated action using middleware-based execution.**
    /// </summary>
    public async Task RunAsync()
    {
        if (DecoratedInstance != null)
        {
            Log.Information($"🚀 Running decorated action sequence for {GetType().Name}");
            await DecoratedInstance.HandleAsync(); // ✅ Middleware executes here
        }
        else
        {
            Log.Information($"🚀 Running normal action sequence for {GetType().Name}");
            await HandleAsync(); // ✅ No middleware, just normal execution
        }
    }

    /// <summary>
    ///     ✅ **Allows actions to access Playwright's Page object if needed.**
    /// </summary>
    public virtual IPage? Page => null; // ✅ Default Page is null for non-Playwright handlers

    /// <summary>
    ///     ✅ **Waits for a condition to be met within a timeout period.**
    /// </summary>
    protected async Task WaitForConditionAsync(Func<Task<bool>> condition, int timeoutMilliseconds = 5000,
        int pollingInterval = 100)
    {
        var stopwatch = Stopwatch.StartNew();
        while (stopwatch.ElapsedMilliseconds < timeoutMilliseconds)
        {
            if (await condition()) return;

            await Task.Delay(pollingInterval);
        }

        throw new TimeoutException("Condition not met within the specified timeout.");
    }

    /// <summary>
    ///     ✅ **Defines the core execution logic for each action.**
    /// </summary>
    protected abstract Task ExecuteAsync();

    /// <summary>
    ///     ✅ **Generic Property for Resolving Any Service from ServiceLocator**
    /// </summary>
    protected T GetService<T>() where T : notnull
    {
        return ServiceLocator.GetService<T>();
    }
}