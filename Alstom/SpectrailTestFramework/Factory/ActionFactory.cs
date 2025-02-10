using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Alstom.Spectrail.Framework.PageObjects;
using Alstom.Spectrail.Framework.Actions;
using Alstom.Spectrail.Framework.Decorators;

namespace Alstom.Spectrail.Framework.Utilities
{
    public class ActionFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ActionFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Get Login Page Instance
        /// </summary>
        public LoginPage GetLoginPage() => _serviceProvider.GetRequiredService<LoginPage>();

        /// <summary>
        /// Create an instance of a Page that inherits BasePage dynamically
        /// </summary>
        public T CreatePage<T>() where T : BasePage
        {
            return (T)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(T));
        }

        /// <summary>
        /// Create an action instance with decorators automatically applied
        /// </summary>
        public T Create<T>() where T : BaseActionHandler
        {
            var action = (T)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(T));

            // ✅ Apply decorators dynamically based on attributes
            IActionHandler decoratedAction = action.ApplyDecorators();

            return (T)decoratedAction;
        }
    }
}