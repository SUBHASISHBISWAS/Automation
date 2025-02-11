using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;

using NUnit.Framework;

using Serilog;

using SpectrailTestFramework.Factory;

using TestContext = NUnit.Framework.TestContext;

namespace SpectrailTests.Hooks;

[SetUpFixture]
public class TestHooks : IAsyncDisposable
{
    private static readonly string ParentDirectory =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpectrailArtifacts");


    private static IPlaywright? _playwright;
    private static IBrowser? _browser;
    private static IBrowserContext? _context;
    private static IPage? _page;
    private static ServiceProvider? _serviceProvider;
    private string? _testName;
    public static ActionFactory? ActionFactory { get; private set; } // ✅ Ensure ActionFactory is accessible

    /// ✅ Asynchronously flush logs and dispose of resources
    public async ValueTask DisposeAsync()
    {
        await Task.Run(Log.CloseAndFlush); // ✅ Flush logs asynchronously
        _serviceProvider?.Dispose();
    }

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
        // ✅ Initialize Playwright and Browser
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        _context = await _browser.NewContextAsync();
        _page = await _context.NewPageAsync();
        // ✅ Setup Dependency Injection (DI) container
        ServiceCollection serviceCollection = new();
        serviceCollection.AddSingleton(_page);
        serviceCollection.AddSingleton<ActionFactory>();
        _serviceProvider = serviceCollection.BuildServiceProvider();
        ActionFactory = _serviceProvider.GetRequiredService<ActionFactory>(); // ✅ Initialize ActionFactory
        Log.Information("✅ Playwright and ActionFactory Initialized.");
    }

    [SetUp]
    public void Setup()
    {
        _testName = TestContext.CurrentContext.Test.Name;
        Log.Information($"🚀 Test {_testName} Started...");
    }

    [TearDown]
    public async Task Cleanup()
    {
        NUnit.Framework.Interfaces.TestStatus testStatus = TestContext.CurrentContext.Result.Outcome.Status;
        Log.Information($"✅ Test {_testName} Completed with Status: {testStatus}");
        await Task.Delay(100); // ✅ Ensure async completion before closing logs
    }

    [OneTimeTearDown]
    public async Task GlobalCleanup()
    {
        Log.Information("✅ Global Test Cleanup Started.");
        if (_browser != null)
        {
            await _browser.CloseAsync();
        }

        if (_serviceProvider != null)
        {
            await DisposeAsync();
        }

        Log.Information("✅ Global Test Cleanup Completed.");
    }
}