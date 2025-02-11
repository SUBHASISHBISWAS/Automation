using Microsoft.Playwright;

using SpectrailTestFramework.Interfaces;

namespace SpectrailTestFramework.Decorators;

/// <summary>
///     Base class for action decorators that extend functionalities dynamically.
/// </summary>
public abstract class BaseActionDecorator(IActionHandler wrappedAction) : IActionHandler
{
    protected readonly IActionHandler _wrappedAction = wrappedAction;

    /// <summary>
    ///     ✅ Expose the original wrapped action for unwrapping.
    /// </summary>
    public IActionHandler WrappedAction => _wrappedAction;

    /// <summary>
    ///     Executes the decorated action.
    /// </summary>
    public virtual async Task HandleAsync()
    {
        await _wrappedAction.HandleAsync();
    }

    /// <summary>
    ///     Supports chaining by passing the next handler to the wrapped action.
    /// </summary>
    public IActionHandler SetNext(IActionHandler nextHandler)
    {
        _wrappedAction.SetNext(nextHandler);
        return this;
    }

    /// <summary>
    ///     Supports applying delays before execution.
    /// </summary>
    public IActionHandler WithDelay(Func<Task> delayFunction)
    {
        _wrappedAction.WithDelay(delayFunction);
        return this;
    }

    /// <summary>
    ///     ✅ Ensures that `RunAsync()` is executed on the wrapped action.
    /// </summary>
    public async Task RunAsync()
    {
        await _wrappedAction.RunAsync();
    }

    /// <summary>
    ///     Exposes the Playwright Page if available from the wrapped action.
    /// </summary>
    public virtual IPage? Page => _wrappedAction.Page; // ✅ Forwards `Page` property dynamically
}