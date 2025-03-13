#region

using NUnit.Allure.Attributes;
using NUnit.Framework;
using SpectrailTests.Handlers;
using SpectrailTests.Hooks;

#endregion

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
        var spectrailNewFeatureHandler = ActionFactory!.GetAction<SpectrailNewFeatureHandler>();
        await spectrailNewFeatureHandler.RunAsync();

        /*
        string testName = TestContext.CurrentContext.Test.Name;
        ExtentReportManager.StartTest(testName);
        ExtentReportManager.LogTestInfo("🚀 Starting Login Test...");

        InstantiationsPageHandler instantiationPageHandler = ActionFactory!.GetAction<InstantiationsPageHandler>();

        //Config?.GetUrl("SpectrailValid");
        await instantiationPageHandler.RunAsync();

        //var handler = ActionFactory!.GetAction<DuplicateVariableHandler>();
        //await handler.RunAsync();
        ExtentReportManager.LogTestPass("✅ Login Test Passed.");
        ExtentReportManager.FlushReport();
        */
    }
}