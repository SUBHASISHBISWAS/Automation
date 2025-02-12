using System;
using System.Threading.Tasks;

using Microsoft.Playwright;

using Serilog;

namespace SpectrailTestFramework.PageObjects;

/// <summary>
/// ✅ **Page Object Model for Login Page.**  
/// ✅ **Handles Playwright interactions (No Business Logic).**  
/// ✅ **Logs UI element visibility & interactions.**  
/// </summary>
public class LoginPage : BasePage
{
    private ILocator _userName => Page.GetByLabel("Username");
    private ILocator _password => Page.GetByLabel("Password");
    private ILocator _submitButton => Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Submit" });
    private ILocator _loggedInSuccessfully => Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Logged In Successfully" });

    public LoginPage(IPage page) : base(page) { }

    /// <summary>
    /// ✅ **Logs UI interactions & handles missing elements gracefully.**
    /// </summary>
    public async Task<LoginPage> Login(string userName, string password)
    {
        Log.Information("🔍 Checking if username and password fields are visible...");

        if (!await _userName.IsVisibleAsync())
        {
            Log.Warning("⚠️ Username field is not visible on the page.");
        }
        if (!await _password.IsVisibleAsync())
        {
            Log.Warning("⚠️ Password field is not visible on the page.");
        }
        if (!await _submitButton.IsVisibleAsync())
        {
            Log.Warning("⚠️ Submit button is not visible on the page.");
        }

        Log.Information("✅ Filling in credentials...");
        await _userName.FillAsync(userName);
        await _password.FillAsync(password);

        Log.Information("🚀 Clicking the login button...");
        await _submitButton.ClickAsync();

        return this;
    }

    /// <summary>
    /// ✅ **Performs login and verifies successful navigation.**
    /// </summary>
    public async Task<LoginPage> LoginAndVerifyLink(string userName, string password)
    {
        Log.Information("🔍 Checking if UI elements are present for Login and Verification...");

        await Login(userName, password);

        Log.Information("🕵️ Waiting for successful navigation...");
        await Page.WaitForURLAsync("https://practicetestautomation.com/logged-in-successfully/");

        Log.Information("✅ Successfully navigated after login.");
        return this;
    }

    /// <summary>
    /// ✅ **Checks if login was successful.**
    /// </summary>
    public async Task<bool> IsLoggedIn()
    {
        bool isLoggedIn = await _loggedInSuccessfully.IsVisibleAsync();
        Log.Information(isLoggedIn ? "✅ Login successful." : "❌ Login failed.");
        return isLoggedIn;
    }
}