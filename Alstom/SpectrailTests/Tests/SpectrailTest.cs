using NUnit.Allure.Attributes;
using NUnit.Framework;

using SpectrailTestFramework.Utilities;

using SpectrailTests.Handlers;
using SpectrailTests.Hooks;

using TestContext = NUnit.Framework.TestContext;

namespace SpectrailTests.Tests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class SpectrailTest : TestHooks
{
    //[Test]
    //[AllureFeature("Login")]
    //public async Task Open_Spectrail_Goto_Project()
    //{
    //    string testName = TestContext.CurrentContext.Test.Name;
    //    ExtentReportManager.StartTest(testName);
    //    ExtentReportManager.LogTestInfo("🚀 Starting Login Test...");

    //    OpenPageHandler? openPageHandler = ActionFactory?.GetAction<OpenPageHandler>()
    //        .WithUrl("https://spectrail-dev.alstom.hub/spectrailvalid/47/Home/Index");

    //    SpectrailPageHandler? spectrailPageHandler = ActionFactory?.GetAction<SpectrailPageHandler>();


    //    await openPageHandler?.SetNextAction(spectrailPageHandler).RunAsync();

    //    ExtentReportManager.LogTestPass("✅ Login Test Passed.");
    //    ExtentReportManager.FlushReport();
    //}

    [Test]
    [AllureFeature("Login")]
    public async Task Open_Instantiation_EditVariable()
    {
        string testName = TestContext.CurrentContext.Test.Name;
        ExtentReportManager.StartTest(testName);
        ExtentReportManager.LogTestInfo("🚀 Starting Login Test...");

        InstantiationsPageHandler instantiationPageHandler = ActionFactory!.GetAction<InstantiationsPageHandler>();

        //Config?.GetUrl("SpectrailValid")
        await instantiationPageHandler.RunAsync();

        ExtentReportManager.LogTestPass("✅ Login Test Passed.");
        ExtentReportManager.FlushReport();
    }
}