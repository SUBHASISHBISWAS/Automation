#region ©COPYRIGHT

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
// FileName: IActionHandler.cs
// ProjectName: Alstom.Spectrail.TestFramework
// Created by SUBHASISH BISWAS On: 2025-03-13
// Updated by SUBHASISH BISWAS On: 2025-03-13
//  ******************************************************************************/

#endregion

#region

using Microsoft.Playwright;

#endregion

namespace Alstom.Spectrail.TestFramework.Interfaces;

/// <summary>
///     ✅ Defines the base contract for actions and decorators.
///     ✅ Supports middleware-style execution and fluent API chaining.
/// </summary>
public interface IActionHandler
{
    /// <summary>
    ///     ✅ Exposes Playwright's Page object if available; otherwise, returns null.
    /// </summary>
    IPage? Page { get; }

    IActionHandler? DecoratedInstance { get; set; }

    /// <summary>
    ///     ✅ Executes the action asynchronously after applying middleware.
    /// </summary>
    Task HandleAsync();

    /// <summary>
    ///     ✅ Sets the next action in the chain and returns the updated handler for fluent chaining.
    /// </summary>
    IActionHandler SetNextAction(IActionHandler nextHandler);

    /// <summary>
    ///     ✅ Adds an optional delay before execution.
    /// </summary>
    IActionHandler WithDelay(Func<Task> delayFunction);

    /// <summary>
    ///     ✅ Registers middleware (decorators) dynamically.
    /// </summary>
    IActionHandler Use(Func<IActionHandler, Func<Task>, Task> middleware);

    /// <summary>
    ///     ✅ Runs the action pipeline, ensuring middleware executes in the correct order.
    /// </summary>
    Task RunAsync();
}