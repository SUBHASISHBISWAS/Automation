using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using ExploringPlaywright.Pages;
namespace SpectrailTests;

    public class UsingSpectrailPageModel : PageTest
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
            
            await _page.GotoAsync("https://spectrail-dev.alstom.hub/spectrailvalid", new PageGotoOptions()
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 0
            });
            await _page.BringToFrontAsync();
        }

       
        
        [Test]
        public async Task CheckSpectrailSystemImportExport()
        {
            var homePage = new SpectrailHomePageModel(_page);
            var isLoggedIn = await homePage.IsProjectVisible();
            Assert.That(isLoggedIn, Is.True);

        
            await homePage.GoToAliviaProjectAndVerifyLink();

            await homePage.GoInputRequirementsAndVerifyLink();


            await Expect(_page.GetByRole(AriaRole.Heading, new() { Name = " Requirements" })).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions()
            {
                Timeout = 5000000
            });
            
            //await Task.Delay(2000);

            //await Page.Locator("//a[@title='Go to projects list']").ClickAsync();

        //- Encapsulates the functionality of waiting for a page navigation event while performing an asynchronous
        //action (e.g., clicking a button).
        //Ensures that the action is executed and navigation is verified before continuing.
        //await Page.GetByRole(AriaRole.Link, new() { Name = "Go to projects list" }).ClickAsync();
        /*
        var response = await _page.RunAndWaitForResponseAsync(
            async () => {  await Page.GetByRole(AriaRole.Link, new() { Name = "Go to projects list" }).ClickAsync();  },
            redirectUrl => redirectUrl.Url.Contains("https://spectrail-dev.alstom.hub/spectrailvalid/2"));
        await response.FinishedAsync();
        */
        // Take a screenshot of the current state
        var screenshotPath = Path.Combine("Data", "screenshot.png");
        await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
        Console.WriteLine($"Screenshot saved at: {screenshotPath}");

        // Access the video file after context closure
        var videoPath = await _page.Video.PathAsync();
        Console.WriteLine($"Video saved at: {videoPath}");
        
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
