#region ©COPYRIGHT

// /*******************************************************************************
//  *   © COPYRIGHT ALSTOM SA 2025 - ALL RIGHTS RESERVED
//  *
//  *  This file is part of the Alstom Spectrail project.
//  *  Unauthorized copying, modification, distribution, or use of this file,
//  *  via any medium, is strictly prohibited.
//  *
//  *  Proprietary and confidential.
//  *  Created and maintained by Alstom for internal use in the Spectrail project.
//  ******************************************************************************/
// 
//  /*******************************************************************************
// AuthorName: SUBHASISH BISWAS
// Email: subhasish.biswas@alstomgroup.com
// FileName: ExtentReportManager.cs
// ProjectName: Alstom.Spectrail.TestFramework
// Created by SUBHASISH BISWAS On: 2025-03-13
// Updated by SUBHASISH BISWAS On: 2025-03-13
//  ******************************************************************************/

#endregion

#region

using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;

#endregion

namespace Alstom.Spectrail.TestFramework.Utilities;

public static class ExtentReportManager
{
    private static readonly string ParentDirectory =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpectrailArtifacts",
            "Reports");

    private static ExtentReports? _extentReports;
    private static readonly AsyncLocal<ExtentTest?> _extentTest = new();

    private static void EnsureInitialized()
    {
        if (_extentReports != null) return; // ✅ Avoid multiple initialization

        Directory.CreateDirectory(ParentDirectory);
        var reportPath = Path.Combine(ParentDirectory, "ExtentReport.html");
        ExtentSparkReporter sparkReporter = new(reportPath);

        // ✅ **Manually Apply Configurations in Code (No XML)**
        sparkReporter.Config.DocumentTitle = "Spectrail Test Report";
        sparkReporter.Config.ReportName = "Automated Test Execution Report";
        sparkReporter.Config.Theme = Theme.Dark;
        sparkReporter.Config.Encoding = "UTF-8";
        sparkReporter.Config.OfflineMode = true; // ✅ Ensures the report works offline

        _extentReports = new ExtentReports();
        _extentReports.AttachReporter(sparkReporter);

        Console.WriteLine("✅ ExtentReports Initialized and Configured in Code.");
    }

    /// ✅ **Starts a new test case report**
    public static void StartTest(string testName)
    {
        EnsureInitialized();
        _extentTest.Value = _extentReports?.CreateTest(testName);
        _extentReports?.Flush();
    }

    /// ✅ **Logs test status**
    public static void LogTestInfo(string message)
    {
        _extentTest.Value?.Info(message);
    }

    public static void LogTestPass(string message)
    {
        _extentTest.Value?.Pass(message);
    }

    public static void LogTestFail(string message, Exception? exception = null)
    {
        EnsureInitialized();
        if (exception != null)
            _extentTest.Value?.Fail($"{message}<br><pre>{exception}</pre>");
        else
            _extentTest.Value?.Fail(message);
    }

    /// ✅ **Saves and flushes the report**
    public static void FlushReport()
    {
        EnsureInitialized();
        _extentReports?.Flush();
    }
}