using System.Diagnostics;
using System.Web;
using Microsoft.Playwright;

namespace ExploringPlaywright;

public class Locators 
{
    
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void OpenGoogle()
    {
        // Encode the query for a URL
        string encodedQuery = HttpUtility.UrlEncode("Playwright Tutorial");

        // Construct the Google search URL
        string googleSearchUrl = $"https://www.google.com/search?q={encodedQuery}";

        // Start a new process to open the default web browser
        Process.Start(new ProcessStartInfo
        {
            FileName = googleSearchUrl,
            UseShellExecute = true // This is important for opening URLs
        });

        Console.WriteLine($"Searching Google for: Playwright Tutorial");

    }
    
    [Test]
    public async Task OpenGoogle_using_recorder()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
        });
        
        const string userDataDir = "Data"; // Specify a folder for user data
        var browserContext = await playwright.Chromium.LaunchPersistentContextAsync(userDataDir,
            new BrowserTypeLaunchPersistentContextOptions
            {
                Headless = false,
                // Enable video recording
                RecordVideoDir = "videos", // Directory to save the video
                RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 } // Optional: Set resolution
                
            });

        var page = browserContext.Pages[0];
        await page.GotoAsync("https://www.google.com/");
        await page.GetByLabel("Search", new() { Exact = true }).ClickAsync();
        await page.GetByLabel("Search", new() { Exact = true }).FillAsync("Playwright Tutorial");
        
        var encodedQuery = HttpUtility.UrlEncode("Playwright Tutorial");
        var googleSearchUrl = $"https://www.google.com/search?q={encodedQuery}";
        await page.GotoAsync(googleSearchUrl);
        
        var video = await page.Video.PathAsync();
        Console.WriteLine($"Video saved at: {video}");
        
        await page.CloseAsync();
    }
    
}