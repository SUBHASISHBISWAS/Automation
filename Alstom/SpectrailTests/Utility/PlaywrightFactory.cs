using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Scrutor;
using SpectrailTestFramework.Actions;
using SpectrailTestFramework.Attributes;
using SpectrailTestFramework.Factory;
using SpectrailTestFramework.Interfaces;
using SpectrailTestFramework.PageObjects;
using SpectrailTestFramework.Decorators;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SpectrailTests.Pages;

namespace SpectrailTests.Utilities
{
    public static class PlaywrightFactory
    {
        public static async Task<ServiceProvider> SetupDependencies()
        {
            ServiceCollection services = new();
            IPlaywright playwright = await Playwright.CreateAsync();

            IBrowser browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 50

            });



            IBrowserContext context = await browser.NewContextAsync(new BrowserNewContextOptions
            {
                RecordVideoDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpectrailArtifacts", "Videos"),
                RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 }
            }); ;

            IPage page = await context.NewPageAsync();

            // ✅ Register Playwright components
            services.AddSingleton(playwright);
            services.AddSingleton(browser);
            services.AddSingleton(context);
            services.AddScoped<IPage>(_ => page);

            // ✅ Register Factories for Handlers & Pages
            services.AddSingleton<IHandlerFactory, HandlerFactory>();
            services.AddSingleton<IPageFactory, PageFactory>();

            services.AddTransient<ActionFactory>();

            // ✅ Register All Page Objects (Lazy Resolved)
            services.Scan(scan => scan
                .FromAssemblyOf<LoginPage>()
                .AddClasses(classes => classes.AssignableTo<IPageObject>()
                    .Where(type => !type.IsAbstract))
                .AsImplementedInterfaces()
                .AsSelf()
                .WithTransientLifetime());

            // ✅ Register All Decorators (Only Decorators Are Pre-Registered)
            services.Scan(scan => scan
                .FromAssemblyOf<LoggingDecorator>()
                .AddClasses(classes => classes
                    .Where(type =>
                        typeof(BaseActionDecorator).IsAssignableFrom(type) &&
                        !type.IsAbstract))
                .As<BaseActionDecorator>()
                .WithTransientLifetime());

            // ✅ Ensure Handlers & Pages are Resolved at Runtime
            RegisterDynamicHandlerResolution(services);

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// ✅ Dynamically Resolves Handlers & Applies Decorators at Runtime
        /// </summary>
        private static void RegisterDynamicHandlerResolution(ServiceCollection services)
        {
            var testAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.FullName!.StartsWith("SpectrailTests"));

            if (testAssembly == null)
            {
                throw new InvalidOperationException("Test assembly 'SpectrailTests' not found.");
            }

            var handlerTypes = testAssembly.GetTypes()
                .Where(t => typeof(IActionHandler).IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var handlerType in handlerTypes)
            {
                var mapsToPageAttr = handlerType.GetCustomAttribute<MapsToPageAttribute>();
                if (mapsToPageAttr != null)
                {
                    services.AddTransient(handlerType, provider =>
                    {
                        // ✅ Dynamically Resolve Page Instance
                        var pageInstance = provider.GetRequiredService(mapsToPageAttr.PageType) as IPageObject;
                        if (pageInstance == null)
                        {
                            throw new InvalidOperationException($"❌ Unable to resolve page object: {mapsToPageAttr.PageType.Name}");
                        }

                        // ✅ Dynamically Resolve Handler
                        var handlerInstance = ActivatorUtilities.CreateInstance(provider, handlerType, pageInstance) as IActionHandler;
                        if (handlerInstance == null)
                        {
                            throw new InvalidOperationException($"❌ Unable to create handler instance: {handlerType.Name}");
                        }

                        // ✅ Apply Decorators at Runtime
                        return ApplyDecorators(provider, handlerInstance);
                    });
                }
            }
        }

        /// <summary>
        /// ✅ Applies Decorators at Runtime to the Handler
        /// </summary>
        private static IActionHandler ApplyDecorators(IServiceProvider provider, IActionHandler actionHandler)
        {
            try
            {


                var decoratedAction = new LoggingDecorator(new ScreenshotDecorator(actionHandler));
                actionHandler.DecoratedInstance = decoratedAction;
                Serilog.Log.Information($"✅ Creating and Applying decorators");
                return actionHandler;
            }
            catch (Exception e)
            {
                Serilog.Log.Error($"❌ Error applying decorators: {e.Message}");
                throw;
            }
        }
    }
}