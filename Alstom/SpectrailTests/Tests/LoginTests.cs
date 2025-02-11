using NUnit.Framework;

using SpectrailTestFramework.Actions;

using SpectrailTests.Hooks;

namespace SpectrailTests.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
public class LoginTests : TestHooks
{
    [Test]
    //[AllureFeature("Login")]
    //[AllureSeverity(Allure.Commons.SeverityLevel.critical)]
    public async Task Test_Login_And_Navigate_With_Video()
    {
        //ExtentReportManager.StartTest("Test_Login_And_Navigate_Parallel");

        OpenPageHandler? openPageAction = ActionFactory?.Create<OpenPageHandler>()
            .WithUrl("https://practicetestautomation.com/practice-test-login"); // Set your actual test URL


        // ✅ Retrieve LoginHandler from ActionFactory
        LoginHandler? loginHandler = ActionFactory?.Create<LoginHandler>()
            .WithUsername("testuser")
            .WithPassword("password123");


        await openPageAction.SetNext(loginHandler).RunAsync();
        // ✅ Now calls `RunAsync()` instead of `ExecuteAsync()`
        //await openPageAction.RunAsync();

        //ExtentReportManager.LogStep("Test completed successfully.");
    }
}