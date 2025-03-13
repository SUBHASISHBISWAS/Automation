#region

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Serilog;
using Alstom.Spectrail.TestFramework.Attributes;
using Alstom.Spectrail.TestFramework.Decorators;
using Alstom.Spectrail.TestFramework.Factory;
using Alstom.Spectrail.TestFramework.Interfaces;
using Alstom.Spectrail.TestFramework.Utilities;
using Alstom.Spectrail.Tests.Pages;

#endregion

namespace Alstom.Spectrail.Tests.Utility;

public static class PlaywrightFactory
{
    public static async Task<ServiceProvider> SetupDependencies()
    {
        ServiceCollection services = new();

        // ✅ Load Configuration
        ConfigHelper configHelper = new();
        services.AddSingleton(configHelper);

        var playwright = await Playwright.CreateAsync();

        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = bool.Parse(configHelper.GetSetting("Headless")),
            SlowMo = int.Parse(configHelper.GetSetting("SlowMo"))
        });

        // ✅ Register Playwright components
        services.AddSingleton(playwright);
        services.AddSingleton(browser);

        // ✅ Register `IBrowserContext` (Recording enabled)
        services.AddSingleton(provider =>
            provider.GetRequiredService<IBrowser>().NewContextAsync(new BrowserNewContextOptions
            {
                RecordVideoDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "SpectrailArtifacts", provider.GetRequiredService<ConfigHelper>().GetSetting("RecordVideoDir")),
                RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 }
            }).GetAwaiter().GetResult()
        );

        // ✅ Register `IPage` with Global Timeout
        services.AddScoped(provider =>
        {
            var context = provider.GetRequiredService<IBrowserContext>();
            var config = provider.GetRequiredService<ConfigHelper>();

            var page = context.NewPageAsync().GetAwaiter().GetResult();

            // ✅ Apply timeout at the **Page** level
            page.SetDefaultTimeout(config.GetIntSetting("Timeout"));

            return page;
        });

        // ✅ Register Factories for Handlers & Pages
        services.AddSingleton<IHandlerFactory, HandlerFactory>();
        services.AddSingleton<IPageFactory, PageFactory>();

        services.AddTransient<ActionFactory>();


        // ✅ Register API Service Factory
        services.AddSingleton<ApiServiceFactory>();

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

        var serviceProvider = services.BuildServiceProvider();
        ServiceLocator.Initialize(serviceProvider); // ✅ Store reference globally
        return serviceProvider;
    }

    /// <summary>
    ///     ✅ Dynamically Resolves Handlers & Applies Decorators at Runtime
    /// </summary>
    private static void RegisterDynamicHandlerResolution(ServiceCollection services)
    {
        var testAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.FullName!.StartsWith("Alstom.Spectrail.Tests"));

        if (testAssembly == null) throw new InvalidOperationException("Test assembly 'Alstom.Spectrail.Tests' not found.");

        var handlerTypes = testAssembly.GetTypes()
            .Where(t => typeof(IActionHandler).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var handlerType in handlerTypes)
        {
            var mapsToPageAttr = handlerType.GetCustomAttribute<MapsToPageAttribute>();
            if (mapsToPageAttr != null)
                services.AddTransient(handlerType, provider =>
                {
                    // ✅ Dynamically Resolve Page Instance
                    var pageInstance = provider.GetRequiredService(mapsToPageAttr.PageType) as IPageObject;
                    if (pageInstance == null)
                        throw new InvalidOperationException(
                            $"❌ Unable to resolve page object: {mapsToPageAttr.PageType.Name}");

                    // ✅ Dynamically Resolve Handler
                    var handlerInstance =
                        ActivatorUtilities.CreateInstance(provider, handlerType, pageInstance) as IActionHandler;
                    if (handlerInstance == null)
                        throw new InvalidOperationException(
                            $"❌ Unable to create handler instance: {handlerType.Name}");

                    // ✅ Apply Decorators at Runtime
                    return ApplyDecorators(provider, handlerInstance);
                });
        }
    }

    /// <summary>
    ///     ✅ Applies Decorators at Runtime to the Handler
    /// </summary>
    private static IActionHandler ApplyDecorators(IServiceProvider provider, IActionHandler actionHandler)
    {
        try
        {
            LoggingDecorator decoratedAction = new(new ScreenshotDecorator(actionHandler));
            actionHandler.DecoratedInstance = decoratedAction;
            Log.Information("\u2705 Creating and Applying decorators");
            return actionHandler;
        }
        catch (Exception e)
        {
            Log.Error($"❌ Error applying decorators: {e.Message}");
            throw;
        }
    }
}