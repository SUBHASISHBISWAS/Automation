using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Playwright;
using SpectrailTestFramework.Interfaces;
using SpectrailTestFramework.Factory;

namespace SpectrailTestFramework.Actions;

/// <summary>
/// ✅ **Base class for all actions, supporting middleware-based execution.**
/// ✅ **Ensures decorators execute dynamically before the actual action.**
/// ✅ **Supports Fluent API for chaining actions and decorators.**
/// </summary>
public abstract class BaseActionHandler : IActionHandler
{
    protected readonly ActionFactory _actionFactory;
    private IActionHandler? _nextHandler;
    private Func<Task>? _delayFunction;
    private readonly List<Func<IActionHandler, Func<Task>, Task>> _middlewares = new(); // ✅ Middleware pipeline

    protected BaseActionHandler(ActionFactory actionFactory)
    {
        _actionFactory = actionFactory ?? throw new ArgumentNullException(nameof(actionFactory));
    }

    /// <summary>
    /// ✅ **Supports Fluent API for chaining actions.**
    /// </summary>
    public IActionHandler SetNextAction(IActionHandler nextHandler)
    {
        if (_nextHandler == null)
        {
            _nextHandler = nextHandler;
            Serilog.Log.Information($"🔗 Chained {this.GetType().Name} ➡ {nextHandler.GetType().Name}");
        }
        else
        {
            _nextHandler.SetNextAction(nextHandler);
        }
        return this;
    }

    /// <summary>
    /// ✅ **Allows adding an optional delay before execution.**
    /// </summary>
    public IActionHandler WithDelay(Func<Task> delayFunction)
    {
        _delayFunction = delayFunction;
        return this;
    }

    /// <summary>
    /// ✅ **Registers middleware dynamically before execution.**
    /// </summary>
    public IActionHandler Use(Func<IActionHandler, Func<Task>, Task> middleware)
    {
        _middlewares.Add(middleware);
        return this;
    }

    /// <summary>
    /// ✅ **Handles the action, ensuring middleware and chaining work correctly.**
    /// </summary>
    public async Task HandleAsync()
    {
        int index = -1;

        async Task Next()
        {
            index++;

            if (index < _middlewares.Count)
            {
                Serilog.Log.Information($"🔹 Running Middleware [{index}] for {this.GetType().Name}");
                await _middlewares[index](this, Next); // ✅ Middleware executes first
            }
            else
            {
                Serilog.Log.Information($"🚀 Executing action: {this.GetType().Name}");
                await ExecuteAsync(); // ✅ Calls the actual action

                if (_nextHandler != null)
                {
                    Serilog.Log.Information($"➡ Moving to next action: {_nextHandler.GetType().Name}");
                    await _nextHandler.HandleAsync(); // ✅ Calls the next action in the chain
                }
            }
        }

        Serilog.Log.Information($"🔄 Starting execution pipeline for {this.GetType().Name}");
        await Next(); // ✅ Start the middleware execution pipeline
    }

    /// <summary>
    /// ✅ **Executes the fully decorated action using middleware-based execution.**
    /// </summary>
    public async Task RunAsync()
    {
        Serilog.Log.Information($"🚀 Running action sequence starting from {GetType().Name}");
        await HandleAsync(); // ✅ Runs the pipeline
    }

    /// <summary>
    /// ✅ **Allows actions to access Playwright's Page object if needed.**
    /// </summary>
    public virtual IPage? Page => null; // ✅ Default Page is null for non-Playwright handlers

    /// <summary>
    /// ✅ **Waits for a condition to be met within a timeout period.**
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
    /// ✅ **Defines the core execution logic for each action.**
    /// </summary>
    protected abstract Task ExecuteAsync();
}            