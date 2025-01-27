using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using ExploringPlaywright.Pages;

namespace ExploringPlaywright
{
    public class UsingPageModel : PageTest
    {
        private IBrowserContext _browserContext;
        private IPage _page;

        [SetUp]
        public async Task Setup()
        {
            const string userDataDir = "Data"; // Directory for user data

            // Launch a persistent browser context
            _browserContext = await Playwright.Chromium.LaunchPersistentContextAsync(
                userDataDir,
                new BrowserTypeLaunchPersistentContextOptions
                {
                    Headless = false,
                    RecordVideoDir = "videos", // Directory for video recording
                    RecordVideoSize = new RecordVideoSize
                    {
                        Width = 1280,
                        Height = 720
                    }
                });

            // Get the first page in the context
            _page = _browserContext.Pages[0];
            
            await _page.GotoAsync("https://practicetestautomation.com/practice-test-login",new PageGotoOptions()
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 0
            });
            await _page.BringToFrontAsync();
        }

        [Test]
        public async Task TestLoginPageModel()
        {

            var loginPage = new LoginPage(_page);
            await loginPage.Login("student", "Password123");
            
            
            // Validate the canonical link
            var canonicalHref = await _page.Locator("link[rel='canonical']").GetAttributeAsync("href");
            var expectedLink = "https://practicetestautomation.com/logged-in-successfully/";

            Console.WriteLine($"Canonical Link Found: {canonicalHref}");
            Assert.That(canonicalHref, Is.EqualTo(expectedLink), $"Expected canonical link '{expectedLink}' not found.");

            var isLoggedIn = await loginPage.IsLoggedIn();
            Assert.That(isLoggedIn, Is.True);

            var response = await _page.RunAndWaitForResponseAsync(
                async () => { await _page.ClickAsync("text=Log out"); },
                redirectUrl => redirectUrl.Url.Contains("https://practicetestautomation.com/practice-test-login"));
            await response.FinishedAsync(); 
            // Take a screenshot of the current state
            var screenshotPath = Path.Combine("Data", "screenshot.png");
            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
            Console.WriteLine($"Screenshot saved at: {screenshotPath}");

            // Access the video file after context closure
            var videoPath = await _page.Video.PathAsync();
            Console.WriteLine($"Video saved at: {videoPath}");
        }
        
        [Test]
        public async Task TestLoginPageModel_2()
        {

            var loginPage = new LoginPage(_page);
            await loginPage.LoginAndVerifyLink("student", "Password123");
            

            var isLoggedIn = await loginPage.IsLoggedIn();
            Assert.That(isLoggedIn, Is.True);

            //- Encapsulates the functionality of waiting for a page navigation event while performing an asynchronous
            //action (e.g., clicking a button).
            //Ensures that the action is executed and navigation is verified before continuing.
            var response = await _page.RunAndWaitForResponseAsync(
                async () => { await _page.ClickAsync("text=Log out"); },
                redirectUrl => redirectUrl.Url.Contains("https://practicetestautomation.com/practice-test-login/") && redirectUrl.Status==200);
           await response.FinishedAsync();
           
            // Take a screenshot of the current state
            var screenshotPath = Path.Combine("Data", "screenshot.png");
            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
            Console.WriteLine($"Screenshot saved at: {screenshotPath}");

            // Access the video file after context closure
            var videoPath = await _page.Video.PathAsync();
            Console.WriteLine($"Video saved at: {videoPath}");
        }
        
        [Test]
        public async Task FlipkartTest()
        {
            
            /*await _page.RouteAsync("*#1#*", async route =>
            {
                if (route.Request.ResourceType == "image")
                    await route.AbortAsync();
                else
                    route.ContinueAsync();
            });*/
            
            _page.Request += (_, request) => Console.WriteLine(request.Method + "---" + request.Url);
            _page.Response += (_, response) => Console.WriteLine(response.Status + "--_" + response.Url) ;
            await _page.GotoAsync("https://www.flipkart.com",new PageGotoOptions()
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 0
            });
            
            
        }

        [TearDown]
        public async Task TearDown()
        {
            // Close the browser context
            if (_browserContext != null)
            {
                await _browserContext.CloseAsync();
            }
        }
    }
}