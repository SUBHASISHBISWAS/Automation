using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;

using SpectrailTestFramework.Actions;
using SpectrailTestFramework.Factory;

namespace SpectrailTestFramework.Utilities;

public static class PlaywrightFactory
{
    public static async Task<ServiceProvider> SetupDependencies()
    {
        ServiceCollection services = new();
        IPlaywright playwright = await Playwright.CreateAsync();

        IBrowser browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false, // ✅ Set to true for CI/CD
            SlowMo = 50 // ✅ Slows down execution for better debugging
        });

        IBrowserContext context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            RecordVideoDir = "videos", // ✅ Enables video recording
            RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 }
        });

        IPage page = await context.NewPageAsync();

        // ✅ Register services in Dependency Injection
        services.AddSingleton(playwright);
        services.AddSingleton(browser);
        services.AddSingleton(context);
        services.AddTransient(_ => page);
        services.AddTransient<ActionFactory>();
        services.AddTransient<LoginHandler>();

        return services.BuildServiceProvider();
    }
}