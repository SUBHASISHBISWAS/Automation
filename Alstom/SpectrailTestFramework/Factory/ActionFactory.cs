using System;

using Microsoft.Extensions.DependencyInjection;

using Serilog;

using SpectrailTestFramework.Actions;
using SpectrailTestFramework.Decorators;
using SpectrailTestFramework.Interfaces;
using SpectrailTestFramework.PageObjects;

namespace SpectrailTestFramework.Factory;

/// <summary>
/// ✅ **Factory for creating actions and pages with dependency injection.**
/// ✅ **Applies middleware decorators dynamically using Fluent API.**
/// </summary>
public class ActionFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ActionFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// ✅ **Creates an instance of a Page that inherits `BasePage` dynamically.**
    /// </summary>
    public T CreatePage<T>() where T : BasePage
    {
        try
        {
            T pageInstance = (T)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(T));

            if (pageInstance.Page == null)
            {
                Log.Error($"[ActionFactory] ❌ Page instance for {typeof(T).Name} is null.");
                throw new InvalidOperationException($"Page instance for {typeof(T).Name} is null.");
            }

            Log.Information($"[ActionFactory] ✅ Successfully created page instance of {typeof(T).Name}");
            return pageInstance;
        }
        catch (Exception ex)
        {
            Log.Error($"[ActionFactory] ❌ Failed to create Page instance of {typeof(T).Name}: {ex.Message}");
            throw new InvalidOperationException($"Unable to create instance of {typeof(T).Name}.", ex);
        }
    }

    /// <summary>
    /// ✅ **Creates an action instance and applies middleware decorators dynamically.**
    /// </summary>
    public T Create<T>() where T : BaseActionHandler
    {
        try
        {
            T actionInstance = (T)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(T));

            // ✅ Wrap with explicit decorator instances
            IActionHandler decoratedAction = new LoggingDecorator(actionInstance);
            decoratedAction = new ScreenshotDecorator(decoratedAction);

            // ✅ Apply middleware at **BaseActionHandler** level, ensuring execution
            actionInstance.Use(LoggingDecorator.Middleware())
                          .Use(ScreenshotDecorator.Middleware());

            Serilog.Log.Information($"[ActionFactory] ✅ Successfully created action instance of {typeof(T).Name}");
            return actionInstance;
        }
        catch (Exception ex)
        {
            Serilog.Log.Error($"[ActionFactory] ❌ Failed to create instance of {typeof(T).Name}: {ex.Message}");
            throw new InvalidOperationException($"Unable to create instance of {typeof(T).Name}.", ex);
        }
    }
}