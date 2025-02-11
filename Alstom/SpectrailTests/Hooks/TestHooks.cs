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
        private static readonly string ParentDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpectrailArtifacts");

        private static readonly AsyncLocal<IPlaywright?> _playwright = new();
        private static readonly AsyncLocal<IBrowser?> _browser = new();
        private static readonly AsyncLocal<IBrowserContext?> _context = new();
        private static readonly AsyncLocal<IPage?> _page = new();
        private static ServiceProvider? _serviceProvider;
        public static ActionFactory? ActionFactory { get; private set; }

        /// <summary>
        /// ✅ Global Test Setup - Initializes Reporting & Playwright.
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

            // ✅ Initialize ExtentReports using ExtentSparkReporter
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

            Log.Information($"🚀 Test {testName} Started...");

            // ✅ Initialize Playwright & Isolated Browser Context Per Test
            _playwright.Value = await Playwright.CreateAsync();
            _browser.Value = await _playwright.Value.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            _context.Value = await _browser.Value.NewContextAsync();
            _page.Value = await _context.Value.NewPageAsync();

            // ✅ Setup Dependency Injection (DI) container
            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(_page.Value);
            serviceCollection.AddSingleton<ActionFactory>();

            _serviceProvider = serviceCollection.BuildServiceProvider();
            ActionFactory = _serviceProvider.GetRequiredService<ActionFactory>();

            Log.Information("✅ Playwright & ActionFactory Initialized.");
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
                Log.Error($"❌ Test {testName} Failed.");
            }
            else
            {
                ExtentReportManager.LogTestPass($"✅ Test {testName} Passed.");
                Log.Information($"✅ Test {testName} Passed.");
            }

            ExtentReportManager.FlushReport();
            await Task.Delay(100);
        }

        /// <summary>
        /// ✅ Global Test Cleanup - Ensures Clean Execution
        /// </summary>
        [OneTimeTearDown]
        public async Task GlobalCleanup()
        {
            Log.Information("✅ Global Test Cleanup Started.");
            ExtentReportManager.FlushReport();

            if (_browser.Value != null)
            {
                await _browser.Value.CloseAsync();
            }

            if (_serviceProvider != null)
            {
                await DisposeAsync();
            }

            Log.Information("✅ Global Test Cleanup Completed.");
        }

        /// <summary>
        /// ✅ Asynchronously flush logs and dispose of resources.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await Task.Run(Log.CloseAndFlush);
            _serviceProvider?.Dispose();
        }
    }
}