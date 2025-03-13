#region

using NUnit.Framework;
using Serilog;
using SpectrailTestFramework.Interfaces;

#endregion

namespace SpectrailTestFramework.Decorators;

public class LoggingDecorator : BaseActionDecorator
{
    private static readonly string ParentLogDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpectrailArtifacts", "Logs");

    private protected readonly string _logFilePath;

    private readonly string _testName = TestContext.CurrentContext.Test.Name;

    public LoggingDecorator(IActionHandler wrappedAction) : base(wrappedAction)
    {
        var testLogDirectory = Path.Combine(ParentLogDirectory, _testName);
        Directory.CreateDirectory(testLogDirectory);
        _logFilePath = Path.Combine(testLogDirectory, "test.log");

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(_logFilePath, rollingInterval: RollingInterval.Infinite)
            .MinimumLevel.Debug()
            .CreateLogger();

        // ✅ Register middleware at the time of instantiation
        Use(Middleware());
    }

    public static Func<IActionHandler, Func<Task>, Task> Middleware()
    {
        return async (handler, next) =>
        {
            Log.Information($"🔹 Starting action: {handler.GetType().Name}");
            try
            {
                await next();
                Log.Information($"✅ Completed action: {handler.GetType().Name}");
            }
            catch (Exception ex)
            {
                Log.Error($"❌ Error in action {handler.GetType().Name}: {ex.Message}");
                throw;
            }
        };
    }
}