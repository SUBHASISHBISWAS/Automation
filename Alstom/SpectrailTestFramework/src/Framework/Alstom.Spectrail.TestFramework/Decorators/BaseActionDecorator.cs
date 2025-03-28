﻿#region ©COPYRIGHT

// /*******************************************************************************
//  *   © COPYRIGHT ALSTOM SA 2025 - ALL RIGHTS RESERVED
//  *
//  *  This file is part of the Alstom Spectrail project.
//  *  Unauthorized copying, modification, distribution, or use of this file,
//  *  via any medium, is strictly prohibited.
//  *
//  *  Proprietary and confidential.
//  *  Created and maintained by Alstom for internal use in the Spectrail project.
//  ******************************************************************************/
// 
//  /*******************************************************************************
// AuthorName: SUBHASISH BISWAS
// Email: subhasish.biswas@alstomgroup.com
// FileName: BaseActionDecorator.cs
// ProjectName: Alstom.Spectrail.TestFramework
// Created by SUBHASISH BISWAS On: 2025-03-13
// Updated by SUBHASISH BISWAS On: 2025-03-13
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.TestFramework.Interfaces;
using Microsoft.Playwright;

#endregion

namespace Alstom.Spectrail.TestFramework.Decorators;

public abstract class BaseActionDecorator : IActionHandler
{
    private readonly List<Func<IActionHandler, Func<Task>, Task>> _middlewares = new(); // ✅ Middleware pipeline
    protected readonly IActionHandler _wrappedAction;

    protected BaseActionDecorator(IActionHandler wrappedAction)
    {
        _wrappedAction = wrappedAction ?? throw new ArgumentNullException(nameof(wrappedAction));

        // ✅ Register decorator-specific middleware dynamically
        Use(Middleware());
    }

    /// <summary>
    ///     ✅ **Exposes the original wrapped action for unwrapping.**
    /// </summary>
    public IActionHandler WrappedAction => _wrappedAction;

    /// <summary>
    ///     ✅ **Adds middleware (decorator logic) dynamically.**
    /// </summary>
    public IActionHandler Use(Func<IActionHandler, Func<Task>, Task> middleware)
    {
        _middlewares.Add(middleware);
        return this;
    }

    /// <summary>
    ///     ✅ **Executes the decorated action within the middleware pipeline.**
    /// </summary>
    public async Task HandleAsync()
    {
        var index = -1;

        async Task Next()
        {
            index++;
            if (index < _middlewares.Count)
                await _middlewares[index](this, Next); // ✅ Executes each middleware in order
            else
                await _wrappedAction.HandleAsync(); // ✅ Calls the actual action
        }

        await Next(); // ✅ Start the middleware execution pipeline
    }

    /// <summary>
    ///     ✅ **Supports chaining by passing the next handler to the wrapped action.**
    /// </summary>
    public IActionHandler SetNextAction(IActionHandler nextHandler)
    {
        _wrappedAction.SetNextAction(nextHandler);
        return this;
    }

    /// <summary>
    ///     ✅ **Supports applying delays before execution.**
    /// </summary>
    public IActionHandler WithDelay(Func<Task> delayFunction)
    {
        _wrappedAction.WithDelay(delayFunction);
        return this;
    }

    /// <summary>
    ///     ✅ **Ensures that `RunAsync()` is executed with middleware-based execution.**
    /// </summary>
    public async Task RunAsync()
    {
        await HandleAsync(); // ✅ Runs the pipeline with decorators
    }

    /// <summary>
    ///     ✅ **Exposes the Playwright Page if available from the wrapped action.**
    /// </summary>
    public virtual IPage? Page => _wrappedAction.Page; // ✅ Forwards `Page` property dynamically

    public IActionHandler? DecoratedInstance { get; set; }

    /// <summary>
    ///     ✅ **Provides default middleware behavior (to be overridden).**
    /// </summary>
    protected virtual Func<IActionHandler, Func<Task>, Task> Middleware()
    {
        return async (handler, next) =>
        {
            await next(); // ✅ Default middleware just calls the next action
        };
    }
}