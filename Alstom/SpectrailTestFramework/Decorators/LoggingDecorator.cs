using NUnit.Framework;

using Serilog;

using SpectrailTestFramework.Interfaces;

namespace SpectrailTestFramework.Decorators;

public class LoggingDecorator : BaseActionDecorator
{
    private readonly string _logDirectory =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpectrailArtifacts",
            "Logs");

    private readonly string _testName = TestContext.CurrentContext.Test.Name;

    public LoggingDecorator(IActionHandler wrappedAction) : base(wrappedAction)
    {
        Directory.CreateDirectory(Path.Combine(_logDirectory, _testName));
        string logFilePath = Path.Combine(_logDirectory, _testName, "test.log");
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Infinite)
            .MinimumLevel.Debug()
            .CreateLogger();
    }

    public override async Task HandleAsync()
    {
        try
        {
            Log.Information($"🔹 Starting action: {_wrappedAction.GetType().Name}");
            await _wrappedAction.HandleAsync();
            Log.Information($"✅ Completed action: {_wrappedAction.GetType().Name}");
        }
        catch (Exception ex)
        {
            Log.Error($"❌ Error in action {_wrappedAction.GetType().Name}: {ex.Message}");
            throw;
        }
    }
}