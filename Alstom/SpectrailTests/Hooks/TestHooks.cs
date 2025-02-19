using Allure.Commons;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;
using NUnit.Framework.Interfaces;

using Serilog;

using SpectrailTestFramework.Factory;
using SpectrailTestFramework.Utilities;

using SpectrailTests.Utility;

using TestContext = NUnit.Framework.TestContext;

namespace SpectrailTests.Hooks;

[SetUpFixture]
public class TestHooks : IAsyncDisposable
{
    private static readonly string ParentDirectory =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpectrailArtifacts");

    private static readonly AsyncLocal<ServiceProvider?> _serviceProvider = new();
    public static ActionFactory? ActionFactory { get; private set; }

    public static ConfigHelper? Config { get; private set; }

    /// <summary>
    ///     ✅ **Asynchronously flush logs and dispose of resources.**
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await Task.Run(Log.CloseAndFlush);
        _serviceProvider.Value?.Dispose();
    }

    /// <summary>
    ///     ✅ **Global Setup - Initializes Logging, Reporting & Playwright.**
    /// </summary>
    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
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

        // ✅ Setup Playwright & DI for test cases
        _serviceProvider.Value = await PlaywrightFactory.SetupDependencies();
        Config = _serviceProvider.Value.GetRequiredService<ConfigHelper>();
        ActionFactory = _serviceProvider.Value.GetRequiredService<ActionFactory>();

        Log.Information("✅ Playwright & Dependency Injection Initialized.");
    }

    /// <summary>
    ///     ✅ **Per-Test Setup - Ensures Parallel Execution.**
    /// </summary>
    [SetUp]
    public void Setup()
    {
        string testName = TestContext.CurrentContext.Test.Name;
        ExtentReportManager.StartTest(testName);
        ExtentReportManager.LogTestInfo($"🚀 Test {testName} Started...");
        Log.Information($"🚀 Test {testName} Started...");
    }

    /// <summary>
    ///     ✅ **Per-Test Cleanup - Handles Reporting & Logs.**
    /// </summary>
    [TearDown]
    public async Task Cleanup()
    {
        string testName = TestContext.CurrentContext.Test.Name;
        TestStatus testStatus = TestContext.CurrentContext.Result.Outcome.Status;

        if (testStatus == TestStatus.Failed)
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
    ///     ✅ **Global Cleanup - Ensures Clean Execution**
    /// </summary>
    [OneTimeTearDown]
    public async Task GlobalCleanup()
    {
        Log.Information("✅ Global Test Cleanup Started.");
        ExtentReportManager.FlushReport();

        if (_serviceProvider.Value != null)
        {
            await DisposeAsync();
        }

        Log.Information("✅ Global Test Cleanup Completed.");
    }
}