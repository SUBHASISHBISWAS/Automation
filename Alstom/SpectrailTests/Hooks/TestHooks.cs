using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

using Allure.Commons;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;

using NUnit.Framework;
using Serilog;
using SpectrailTestFramework.Factory;
using SpectrailTestFramework.Utilities;

using TestContext = NUnit.Framework.TestContext;

namespace SpectrailTests.Hooks
{
    [SetUpFixture]
    public class TestHooks : IAsyncDisposable
    {
        private static readonly string ParentDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpectrailArtifacts");

        private static readonly AsyncLocal<IPlaywright?> _playwright = new();
        private static readonly AsyncLocal<IBrowser?> _browser = new();
        private static readonly AsyncLocal<IBrowserContext?> _context = new();
        private static readonly AsyncLocal<IPage?> _page = new();
        private static readonly AsyncLocal<ServiceProvider?> _serviceProvider = new();
        public static ActionFactory? ActionFactory { get; private set; }

        /// <summary>
        /// ✅ Global Test Setup - Initializes Logging, Reporting & Playwright.
        /// </summary>
        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            // ✅ Configure global logging
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(ParentDirectory, "GlobalLogs.log"), rollingInterval: RollingInterval.Day)
                .MinimumLevel.Debug()
                .CreateLogger();

            Log.Information("🚀 Global Test Setup Started...");

            // ✅ Initialize ExtentReports for structured logging
            ExtentReportManager.FlushReport();

            // ✅ Initialize Allure Reporting
            AllureLifecycle.Instance.CleanupResultDirectory();
        }

        /// <summary>
        /// ✅ Per-Test Setup - Ensures Parallel Execution
        /// </summary>
        [SetUp]
        public async Task Setup()
        {
            string testName = TestContext.CurrentContext.Test.Name;
            ExtentReportManager.StartTest(testName);
            ExtentReportManager.LogTestInfo($"🚀 Test {testName} Started...");
            Serilog.Log.Information($"🚀 Test {testName} Started...");

            // ✅ Initialize Playwright & Isolated Browser Context Per Test
            _playwright.Value = await Playwright.CreateAsync();
            _browser.Value = await _playwright.Value.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });

            _context.Value = await _browser.Value.NewContextAsync();
            _page.Value = await _context.Value.NewPageAsync();

            // ✅ Setup Dependency Injection (DI) container
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_page.Value);
            serviceCollection.AddSingleton<ActionFactory>();

            _serviceProvider.Value = serviceCollection.BuildServiceProvider();
            ActionFactory = _serviceProvider.Value.GetRequiredService<ActionFactory>();

            Serilog.Log.Information("✅ Playwright & ActionFactory Initialized.");
        }

        /// <summary>
        /// ✅ Per-Test Cleanup - Handles Reporting & Logs
        /// </summary>
        [TearDown]
        public async Task Cleanup()
        {
            string testName = TestContext.CurrentContext.Test.Name;
            NUnit.Framework.Interfaces.TestStatus testStatus = TestContext.CurrentContext.Result.Outcome.Status;

            if (testStatus == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                ExtentReportManager.LogTestFail($"❌ Test {testName} Failed.");
                Serilog.Log.Error($"❌ Test {testName} Failed.");
            }
            else
            {
                ExtentReportManager.LogTestPass($"✅ Test {testName} Passed.");
                Serilog.Log.Information($"✅ Test {testName} Passed.");
            }

            ExtentReportManager.FlushReport();

            // ✅ Cleanup Playwright context to avoid memory leaks
            if (_context.Value != null)
            {
                await _context.Value.CloseAsync();
            }

            await Task.Delay(100);
        }

        /// <summary>
        /// ✅ Global Test Cleanup - Ensures Clean Execution
        /// </summary>
        [OneTimeTearDown]
        public async Task GlobalCleanup()
        {
            Serilog.Log.Information("✅ Global Test Cleanup Started.");
            ExtentReportManager.FlushReport();

            if (_browser.Value != null)
            {
                await _browser.Value.CloseAsync();
            }

            if (_serviceProvider.Value != null)
            {
                await DisposeAsync();
            }

            Serilog.Log.Information("✅ Global Test Cleanup Completed.");
        }

        /// <summary>
        /// ✅ Asynchronously flush logs and dispose of resources.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await Task.Run(Serilog.Log.CloseAndFlush);
            _serviceProvider.Value?.Dispose();
        }
    }
}