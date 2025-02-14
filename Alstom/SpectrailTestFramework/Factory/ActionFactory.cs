using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SpectrailTestFramework.Interfaces;
using SpectrailTestFramework.Decorators;
using System;
using System.Linq;
using System.Collections.Generic;
using SpectrailTestFramework.Actions;

namespace SpectrailTestFramework.Factory
{
    public class ActionFactory 
    {
        private readonly IServiceProvider _serviceProvider;

        public ActionFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// ✅ **Creates and returns an instance of the requested handler with decorators applied.**
        /// </summary>
        public T GetAction<T>() where T : BaseActionHandler
        {
            try
            {
                // ✅ Retrieve the base handler instance from DI
                var actionInstance = _serviceProvider.GetRequiredService<T>();

                // ✅ Ensure correct return type
                return actionInstance ?? throw new InvalidOperationException(
                    $"❌ Decorator wrapping error: {actionInstance.GetType().Name} cannot be cast to {typeof(T).Name}");
            }
            catch (Exception ex)
            {
                Log.Error($"[ActionFactory] ❌ Failed to create instance of {typeof(T).Name}: {ex.Message}");
                throw new InvalidOperationException($"Unable to create instance of {typeof(T).Name}.", ex);
            }
        }

        
    }
}