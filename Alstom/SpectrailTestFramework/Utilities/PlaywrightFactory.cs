using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using Alstom.Spectrail.Framework.PageObjects;
using Alstom.Spectrail.Framework.Utilities;
using Alstom.Spectrail.Framework.Decorators;

namespace Alstom.Spectrail.Framework.Utilities
{
    public static class PlaywrightFactory
    {
        public static async Task<ServiceProvider> SetupDependencies()
        {
            var services = new ServiceCollection();
            var playwright = await Playwright.CreateAsync();

            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,  // ✅ Set to true for CI/CD
                SlowMo = 50        // ✅ Slows down execution for better debugging
            });

            var context = await browser.NewContextAsync(new BrowserNewContextOptions
            {
                RecordVideoDir = "videos", // ✅ Enables video recording
                RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 }
            });

            var page = await context.NewPageAsync();

            // ✅ Register services in Dependency Injection
            services.AddSingleton(playwright);
            services.AddSingleton(browser);
            services.AddSingleton(context);
            services.AddTransient<IPage>(_ => page);
            services.AddTransient<LoginPage>();
            services.AddTransient<ActionFactory>();

            return services.BuildServiceProvider();
        }
    }
}