using System;
using Microsoft.Extensions.DependencyInjection;
using Alstom.Spectrail.Framework.Actions;
using Alstom.Spectrail.Framework.Decorators;
using Serilog;
using Alstom.Spectrail.Framework.PageObjects;

namespace Alstom.Spectrail.Framework.Utilities
{
    public class ActionFactory(IServiceProvider serviceProvider)
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        /// <summary>
        /// Create an instance of a Page that inherits BasePage dynamically.
        /// Ensures Playwright dependencies are properly injected.
        /// </summary>
        public T CreatePage<T>() where T : BasePage
        {
            try
            {
                // ✅ Ensure Playwright services are registered
                var pageInstance = (T)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(T));

                if (pageInstance.Page == null)
                {
                    Log.Error($"[ActionFactory] Page instance for {typeof(T).Name} is null. Ensure Playwright is correctly initialized.");
                    throw new InvalidOperationException($"Page instance for {typeof(T).Name} is null.");
                }

                Log.Information($"[ActionFactory] Successfully created page instance of {typeof(T).Name}");
                return pageInstance;
            }
            catch (Exception ex)
            {
                Log.Error($"[ActionFactory] Failed to create Page instance of {typeof(T).Name}: {ex.Message}");
                throw new InvalidOperationException($"Unable to create instance of {typeof(T).Name}. Ensure it is registered in DI.", ex);
            }
        }
        /// <summary>
        /// Create an action instance with decorators automatically applied.
        /// </summary>
        public T Create<T>() where T : BaseActionHandler
        {
            try
            {
                var actionInstance = (T)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(T));

                // ✅ Apply decorators while ensuring type safety
                return actionInstance.ApplyDecorators<T>();
            }
            catch (Exception ex)
            {
                Log.Error($"[ActionFactory] Failed to create instance of {typeof(T).Name}: {ex.Message}");
                throw new InvalidOperationException($"Unable to create instance of {typeof(T).Name}. Ensure it is registered in DI.", ex);
            }
        }
    }
}