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
    [AllureFeature("Login")]
    [AllureSeverity(Allure.Commons.SeverityLevel.critical)]
    public async Task Test_Login_And_Navigate_With_Video()
    {
        ExtentReportManager.StartTest("Test_Login_And_Navigate_Parallel");

        var openPage = new LoggingDecorator(new OpenPageHandler(Page, "https://example.com"));
        var login = new ScreenshotDecorator(new VideoDecorator(new LoginHandler(LoginPage, "testuser", "password123"), Page), Page);
        // Chain actions together
        openPage.SetNext(login);
        // Start execution from first handler
        await openPage.HandleAsync();
        ExtentReportManager.LogStep("Test completed successfully.");
    }
}