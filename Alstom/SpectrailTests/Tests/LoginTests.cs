using Alstom.Spectrail.Framework.Actions;
using Alstom.Spectrail.Framework.Decorators;
using Alstom.Spectrail.Framework.PageObjects;
using Alstom.Spectrail.Tests.Hooks;

using NUnit.Allure.Attributes;
using NUnit.Framework;

using System.Threading.Tasks;
[TestFixture, Parallelizable(ParallelScope.Self)]
public class LoginTests : TestHooks
{
    [Test]
    //[AllureFeature("Login")]
   //[AllureSeverity(Allure.Commons.SeverityLevel.critical)]
    public async Task Test_Login_And_Navigate_With_Video()
    {
        //ExtentReportManager.StartTest("Test_Login_And_Navigate_Parallel");

        var openPageAction = _actionFactory.Create<OpenPageHandler>()
            .WithUrl("https://practicetestautomation.com/practice-test-login"); // Set your actual test URL

       
        // ✅ Retrieve LoginHandler from ActionFactory
        var loginHandler = _actionFactory.Create<LoginHandler>()
            .WithUsername("testuser")
            .WithPassword("password123");


        await openPageAction.SetNext(loginHandler).RunAsync();
        // ✅ Now calls `RunAsync()` instead of `ExecuteAsync()`
        //await openPageAction.RunAsync();

        //ExtentReportManager.LogStep("Test completed successfully.");
    }
}