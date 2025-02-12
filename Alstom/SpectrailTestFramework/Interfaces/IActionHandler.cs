using Microsoft.Playwright;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpectrailTestFramework.Interfaces;

/// <summary>
/// ✅ Defines the base contract for actions and decorators.
/// ✅ Supports middleware-style execution and fluent API chaining.
/// </summary>
public interface IActionHandler
{
    /// <summary>
    /// ✅ Exposes Playwright's Page object if available; otherwise, returns null.
    /// </summary>
    IPage? Page { get; }

    /// <summary>
    /// ✅ Executes the action asynchronously after applying middleware.
    /// </summary>
    Task HandleAsync();

    /// <summary>
    /// ✅ Sets the next action in the chain and returns the updated handler for fluent chaining.
    /// </summary>
    IActionHandler SetNextAction(IActionHandler nextHandler);

    /// <summary>
    /// ✅ Adds an optional delay before execution.
    /// </summary>
    IActionHandler WithDelay(Func<Task> delayFunction);

    /// <summary>
    /// ✅ Registers middleware (decorators) dynamically.
    /// </summary>
    IActionHandler Use(Func<IActionHandler, Func<Task>, Task> middleware);

    /// <summary>
    /// ✅ Runs the action pipeline, ensuring middleware executes in the correct order.
    /// </summary>
    Task RunAsync();
}