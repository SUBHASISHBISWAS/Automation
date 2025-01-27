using Microsoft.Playwright;

namespace ExploringPlaywright;
/*
public class LearningBasics
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Test1()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions()
        {
            Headless = false,
        });

        // Create a new browser context
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync("https://playwright.dev/");
        await page.ClickAsync("text=Doc");
        Console.WriteLine("Press Enter to close the browser...");
        Console.ReadLine();

    }

   
    [Test]
    public async Task Test2()
    {
        using var playwright = await Playwright.CreateAsync();
        const string userDataDir = "Data"; // Specify a folder for user data
        var browserContext = await playwright.Chromium.LaunchPersistentContextAsync(userDataDir,
            new BrowserTypeLaunchPersistentContextOptions
            {
                Headless = false,
                // Enable video recording
                RecordVideoDir = "videos", // Directory to save the video
                RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 } // Optional: Set resolution
                
            });

        var page = browserContext.Pages[0]; // Get the first page in the context
        await page.GotoAsync("https://playwright.dev/");
        await page.BringToFrontAsync();
        await Task.Delay(2000);

        // Navigate to "Community" link
        await page.ClickAsync("text=Community");
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded); // Ensure the page fully loads
        

        var canonicalHref = await page.Locator("link[rel='canonical']").GetAttributeAsync("href");
        var expectedLink = "https://playwright.dev/community/welcome";

        Console.WriteLine($"Canonical Link Found: {canonicalHref}");
        Assert.That(canonicalHref, Is.EqualTo(expectedLink), $"Expected canonical link '{expectedLink}' not found.");




        // Interact with search
        await page.Keyboard.DownAsync("Meta"); // Press 'Command' (⌘) on macOS
        await page.Keyboard.PressAsync("k"); // Press 'K'
        await page.Keyboard.UpAsync("Meta"); // Release 'Command' (⌘)
        await Task.Delay(1000);

        
        
        // Fill the search input and press Enter
        await page.FillAsync("input[type='search']", "How to use Playwright!");
        // Wait for the search box to reflect the input (optional if instant)
        await page.Locator("input[type='search']").WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Attached // Ensure the input is attached to the DOM
        });
       
        await Task.Delay(2000);
        await page.Keyboard.PressAsync("Enter");
        await Task.Delay(2000);
        await page.Locator("input[type='search']").WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Hidden // Ensure the input is attached to the DOM
        });

        // Take a screenshot of the current state
        await page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "Data/screenshot.png",
        });

        // Access the video file after context closure
        var video = await page.Video.PathAsync();
        Console.WriteLine($"Video saved at: {video}");
        
        await browserContext.CloseAsync();
    }
}
*/