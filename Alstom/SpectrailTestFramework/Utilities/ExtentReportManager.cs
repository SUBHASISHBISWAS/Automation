using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

using NUnit.Framework;

using System;
using System.IO;
using System.Threading;
public static class ExtentReportManager
{
    private static readonly string ReportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestResults");
    private static ExtentReports _extentReports;
    private static readonly ThreadLocal<ExtentTest> _currentTest = new();
    public static void InitializeReport()
    {
        try
        {
            if (!Directory.Exists(ReportPath))
            {
                Directory.CreateDirectory(ReportPath);
            }
            var sparkReporter = new ExtentSparkReporter(Path.Combine(ReportPath, "ExtentReport.html"));
            _extentReports = new ExtentReports();
            _extentReports.AttachReporter(sparkReporter);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing report: {ex.Message}");
        }
    }
    public static void StartTest(string testName)
    {
        if (_extentReports == null)
        {
            throw new InvalidOperationException("ExtentReports has not been initialized. Call InitializeReport() first.");
        }
        _currentTest.Value = _extentReports.CreateTest(testName);
    }
    public static void LogStep(string stepDescription, Status status = Status.Info)
    {
        var test = _currentTest.Value;
        if (test == null)
        {
            throw new InvalidOperationException("Test has not been started. Call StartTest() first.");
        }
        test.Log(status, stepDescription);
    }
    public static void AttachScreenshot(string filePath)
    {
        var test = _currentTest.Value;
        if (test == null)
        {
            throw new InvalidOperationException("Test has not been started. Call StartTest() first.");
        }
        if (!File.Exists(filePath))
        {
            test.Log(Status.Warning, $"Screenshot file not found: {filePath}");
            return;
        }
        test.AddScreenCaptureFromPath(filePath);
    }
    public static void FinalizeReport()
    {
        if (_extentReports != null)
        {
            _extentReports.Flush();
        }
    }
}