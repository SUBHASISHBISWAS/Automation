using Microsoft.Extensions.DependencyInjection;

using Serilog;

using SpectrailTestFramework.Actions;
using SpectrailTestFramework.Decorators;
using SpectrailTestFramework.Interfaces;
using SpectrailTestFramework.PageObjects;

namespace SpectrailTestFramework.Factory;

public class ActionFactory(IServiceProvider serviceProvider)
{
    private readonly IServiceProvider _serviceProvider =
        serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    /// <summary>
    /// ✅ **Create an instance of a Page that inherits BasePage dynamically.**
    /// </summary>
    public T CreatePage<T>() where T : BasePage
    {
        try
        {
            T pageInstance = (T)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(T));

            if (pageInstance.Page == null)
            {
                Log.Error($"[ActionFactory] Page instance for {typeof(T).Name} is null.");
                throw new InvalidOperationException($"Page instance for {typeof(T).Name} is null.");
            }

            Log.Information($"[ActionFactory] Successfully created page instance of {typeof(T).Name}");
            return pageInstance;
        }
        catch (Exception ex)
        {
            Log.Error($"[ActionFactory] Failed to create Page instance of {typeof(T).Name}: {ex.Message}");
            throw new InvalidOperationException($"Unable to create instance of {typeof(T).Name}.", ex);
        }
    }

    /// <summary>
    /// ✅ **Create an action instance with decorators automatically applied.**
    /// </summary>
    public T Create<T>() where T : BaseActionHandler
    {
        try
        {
            T actionInstance = (T)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(T));

            // ✅ Apply decorators dynamically
            IActionHandler decoratedAction = actionInstance.ApplyDecorators();

            // ✅ Unwrap the action if needed
            while (decoratedAction is BaseActionDecorator decorator)
            {
                decoratedAction = decorator.WrappedAction;
            }

            return decoratedAction as T ?? throw new InvalidOperationException(
                $"❌ Decorator wrapping error: {decoratedAction.GetType().Name} cannot be cast to {typeof(T).Name}");
        }
        catch (Exception ex)
        {
            Log.Error($"[ActionFactory] Failed to create instance of {typeof(T).Name}: {ex.Message}");
            throw new InvalidOperationException($"Unable to create instance of {typeof(T).Name}.", ex);
        }
    }
}