using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ExploringPlaywright
{
    public class PlaywrightTest : PageTest
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
            
            await _page.GotoAsync("https://playwright.dev/",new PageGotoOptions()
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 0
            });
            await _page.BringToFrontAsync();
        }

        [Test]
        public async Task TestPlaywrightSite()
        {
            // Navigate to the Playwright site
            

            // Navigate to "Community" link
            await _page.ClickAsync("text=Community");
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded); // Ensure the page fully loads

            // Validate the canonical link
            var canonicalHref = await _page.Locator("link[rel='canonical']").GetAttributeAsync("href");
            var expectedLink = "https://playwright.dev/community/welcome";

            Console.WriteLine($"Canonical Link Found: {canonicalHref}");
            Assert.That(canonicalHref, Is.EqualTo(expectedLink), $"Expected canonical link '{expectedLink}' not found.");

            //_page.SetDefaultTimeout(10000);
            await Expect(_page.Locator("h1:has-text('Welcome')")).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions()
            {
                Timeout = 5000000
            });

            // Interact with search
            string key = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "Meta" : "Control";

            await _page.Keyboard.DownAsync(key); // Press the appropriate key
            await _page.Keyboard.PressAsync("k"); // Press 'K'
            await _page.Keyboard.UpAsync(key);   // Release the appropriate key
            await Task.Delay(1000);
            
            
            // Fill the search input and press Enter
            await _page.FillAsync("input[type='search']", "How to use Playwright!");
            // Wait for the search box to reflect the input (optional if instant)
            await _page.Locator("input[type='search']").WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Attached // Ensure the input is attached to the DOM
            });
            
            
            // Wait for the search results
            await Page.Keyboard.PressAsync("Enter");
            await Task.Delay(1000);
            await Page.Locator("input[type='search']").WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Hidden // Ensure the input is attached to the DOM
            });

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
}