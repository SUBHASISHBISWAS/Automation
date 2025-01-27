using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace ExploringPlaywright;

public class UsingNunitLearningBasics:PageTest
{
    [SetUp]
    public async Task Setup()
    {
        var browserChannel = TestContext.Parameters.Get("BrowserChannel", "chromium");
        var headless = bool.Parse(TestContext.Parameters.Get("Headless", "true"));
        var slowMo = int.Parse(TestContext.Parameters.Get("SlowMo", "0"));
        
        await Page.GotoAsync("https://playwright.dev/",new PageGotoOptions()
        {
            WaitUntil = WaitUntilState.DOMContentLoaded,
            Timeout = 0
        });
    }
    public override BrowserNewContextOptions ContextOptions()
    {
        // Create a directory for videos if it doesn't exist
        var videoDir = Path.Combine(Directory.GetCurrentDirectory(), "videos");
        if (!Directory.Exists(videoDir))
        {
            Directory.CreateDirectory(videoDir);
        }

        return new BrowserNewContextOptions
        {
            RecordVideoDir = videoDir, // Specify video directory
            RecordVideoSize = new()
            {
                Width = 1280, // Set video width
                Height = 720 // Set video height
            },
            
        };
        
    }
   
    [Test]
    public async Task Test2()
    {
        
        await Page.BringToFrontAsync();
        await Task.Delay(2000);

        // Navigate to "Community" link
        await Page.ClickAsync("text=Community");
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded); // Ensure the page fully loads
        
        var canonicalHref = await Page.Locator("link[rel='canonical']").GetAttributeAsync("href");
        var expectedLink = "https://playwright.dev/community/welcome";
        Console.WriteLine($"Canonical Link Found: {canonicalHref}");
        Assert.That(canonicalHref, Is.EqualTo(expectedLink), $"Expected canonical link '{expectedLink}' not found.");




        // Interact with search
        await Page.Keyboard.DownAsync("Meta"); // Press 'Command' (⌘) on macOS
        await Page.Keyboard.PressAsync("k"); // Press 'K'
        await Page.Keyboard.UpAsync("Meta"); // Release 'Command' (⌘)
        await Task.Delay(1000);

        
        
        // Fill the search input and press Enter
        await Page.FillAsync("input[type='search']", "How to use Playwright!");
        // Wait for the search box to reflect the input (optional if instant)
        await Page.Locator("input[type='search']").WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Attached // Ensure the input is attached to the DOM
        });
       
        await Task.Delay(2000);
        await Page.Keyboard.PressAsync("Enter");
        await Task.Delay(2000);
        await Page.Locator("input[type='search']").WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Hidden // Ensure the input is attached to the DOM
        });

        // Take a screenshot of the current state
        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "Data/screenshot.png",
        });

        // Access the video file after context closure
        var video = await Page.Video.PathAsync();
        Console.WriteLine($"Video saved at: {video}");
        
        await Page.CloseAsync();
    }
    
    [Test]
    public async Task MyTest()
    {
        await Page.GotoAsync("https://playwright.dev/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Community" }).ClickAsync();
        await Page.GetByLabel("Search (Command+K)").ClickAsync();
        await Page.GetByPlaceholder("Search docs").FillAsync("Locator");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Locators", Exact = true }).ClickAsync();
    }
}