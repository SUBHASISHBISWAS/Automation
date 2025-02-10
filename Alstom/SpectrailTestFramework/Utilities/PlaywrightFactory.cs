using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;

using System.Threading.Tasks;
using Alstom.Spectrail.Framework.PageObjects;

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
                Headless = false,  // Enable UI mode for debugging
                Args = new[] { "--disable-dev-shm-usage", "--no-sandbox" } // Prevent crashes in CI
            });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions
            {
                RecordVideoSize = new() { Width = 1280, Height = 720 },  // Set video resolution
                RecordVideoDir = "Videos"  // Directory where videos will be stored
            });
            services.AddSingleton(playwright);
            services.AddSingleton(browser);
            services.AddSingleton(context);
            services.AddTransient<IPage>(sp => context.NewPageAsync().Result);
            services.AddTransient<LoginPage>();
            return services.BuildServiceProvider();
        }
    }
}