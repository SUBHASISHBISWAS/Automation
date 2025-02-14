using NUnit.Allure.Attributes;
using NUnit.Framework;
using SpectrailTestFramework.Actions;
using SpectrailTestFramework.Utilities;
using SpectrailTests.Hooks;
using TestContext = NUnit.Framework.TestContext;

namespace SpectrailTests.Tests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class SpectrailTest : TestHooks
{
    [Test]
    [AllureFeature("Login")]
    public async Task Open_Spectrail_Goto_Project()
    {
        var testName = TestContext.CurrentContext.Test.Name;
        ExtentReportManager.StartTest(testName);
        ExtentReportManager.LogTestInfo("🚀 Starting Login Test...");

        var openPageHandler = ActionFactory?.GetAction<OpenPageHandler>()
            .WithUrl("https://spectrail-dev.alstom.hub/spectrailvalid");

        var spectrailPageHandler = ActionFactory?.GetAction<SpectrailPageHandler>();
           

        await openPageHandler?.SetNextAction(spectrailPageHandler).RunAsync();

        ExtentReportManager.LogTestPass("✅ Login Test Passed.");
        ExtentReportManager.FlushReport();
    }

    
}