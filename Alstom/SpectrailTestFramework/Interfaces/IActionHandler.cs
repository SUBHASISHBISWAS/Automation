using System;
using System.Threading.Tasks;

using Microsoft.Playwright;

namespace Alstom.Spectrail.Framework.Actions
{
    public interface IActionHandler
    {
        /// <summary>
        /// Executes the action asynchronously.
        /// </summary>
        Task HandleAsync();

        /// <summary>
        /// Sets the next action in the chain and returns the updated handler for fluent chaining.
        /// </summary>
        IActionHandler SetNext(IActionHandler nextHandler);

        /// <summary>
        /// Adds an optional delay before execution.
        /// </summary>
        IActionHandler WithDelay(Func<Task> delayFunction);

        /// <summary>
        /// Exposes Playwright's Page object if available. Otherwise, returns null.
        /// </summary>
        IPage? Page { get; } // ✅ Now defined as a read-only property

        Task RunAsync();
    }
}