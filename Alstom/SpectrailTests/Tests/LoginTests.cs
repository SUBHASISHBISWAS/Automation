using NUnit.Allure.Attributes;
using NUnit.Framework;
using SpectrailTestFramework.Actions;
using SpectrailTestFramework.Utilities;
using SpectrailTests.Hooks;
using TestContext = NUnit.Framework.TestContext;

namespace SpectrailTests.Tests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class LoginTests : TestHooks
{
    [Test]
    [AllureFeature("Login")]
    public async Task Test_Login_And_Navigate_With_Logging()
    {
        var testName = TestContext.CurrentContext.Test.Name;
        ExtentReportManager.StartTest(testName);
        ExtentReportManager.LogTestInfo("🚀 Starting Login Test...");

        var openPageAction = ActionFactory?.Create<OpenPageHandler>()
            .WithUrl("https://practicetestautomation.com/practice-test-login");

        var loginHandler = ActionFactory?.Create<LoginHandler>()
            .WithUsername("student")
            .WithPassword("Password123");

        //loginHandler.RunAsync();
        if (openPageAction != null && loginHandler != null) await openPageAction.SetNext(loginHandler).RunAsync();

        ExtentReportManager.LogTestPass("✅ Login Test Passed.");
        ExtentReportManager.FlushReport();
    }
}