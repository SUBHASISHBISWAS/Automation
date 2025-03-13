#region

using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SpectrailTestFramework.Actions;

#endregion

namespace SpectrailTestFramework.Factory;

public class ActionFactory(IServiceProvider serviceProvider)
{
    private readonly IServiceProvider _serviceProvider =
        serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    /// <summary>
    ///     ✅ **Creates and returns an instance of the requested handler with decorators applied.**
    /// </summary>
    public T GetAction<T>() where T : BaseActionHandler
    {
        try
        {
            // ✅ Retrieve the base handler instance from DI
            var actionInstance = _serviceProvider.GetRequiredService<T>();

            // ✅ Ensure correct return type
            return actionInstance ?? throw new InvalidOperationException(
                $"❌ Decorator wrapping error: {actionInstance?.GetType().Name} cannot be cast to {typeof(T).Name}");
        }
        catch (Exception ex)
        {
            Log.Error($"[ActionFactory] ❌ Failed to create instance of {typeof(T).Name}: {ex.Message}");
            throw new InvalidOperationException($"Unable to create instance of {typeof(T).Name}.", ex);
        }
    }
}