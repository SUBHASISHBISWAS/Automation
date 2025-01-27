using Microsoft.Playwright;

namespace ExploringPlaywright;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task  Test1()
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
    public async Task  Test2()
    {
        using var playwright = await Playwright.CreateAsync();
        var userDataDir = "/Users/subhasishbiswas/GIT/Interstellar/Automation/Playwright/ExploringPlaywright/Data"; // Specify a folder for user data
        var browserContext = await playwright.Webkit.LaunchPersistentContextAsync(userDataDir, new BrowserTypeLaunchPersistentContextOptions
        {
            Headless = false
        });

        var page = browserContext.Pages[0]; // Get the first page in the context
        await page.GotoAsync("https://playwright.dev/");
        await page.ClickAsync("text=Doc");

    }
}