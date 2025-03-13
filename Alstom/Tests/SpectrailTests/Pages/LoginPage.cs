#region

using Microsoft.Playwright;
using SpectrailTestFramework.Interfaces;
using SpectrailTestFramework.PageObjects;

#endregion

namespace SpectrailTests.Pages;

/// <summary>
///     ✅ **Page Object Model for Login Page.**
///     ✅ **Ensures elements are loaded before interactions.**
/// </summary>
public class LoginPage(IPage page, IPageFactory pageFactory) : BasePage(page)
{
    /// <summary>
    ///     ✅ **Constructor that accepts an `IPageObject`**
    /// </summary>
    private readonly IPageFactory _pageFactory = pageFactory;

    private ILocator _userName => Page.GetByLabel("Username");
    private ILocator _password => Page.GetByLabel("Password");
    private ILocator _submitButton => Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Submit" });

    private ILocator _loggedInSuccessfully =>
        Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Logged In Successfully" });

    /// <summary>
    ///     ✅ **Waits for the login form to be visible**
    /// </summary>
    public async Task<bool> IsLoginFormVisible()
    {
        return await _userName.IsVisibleAsync() &&
               await _password.IsVisibleAsync() &&
               await _submitButton.IsVisibleAsync();
    }

    /// <summary>
    ///     ✅ **Performs a login action**
    /// </summary>
    public async Task Login(string userName, string password)
    {
        await _userName.FillAsync(userName);
        await _password.FillAsync(password);
        await _submitButton.ClickAsync();
    }

    /// <summary>
    ///     ✅ **Performs login and verifies successful navigation**
    /// </summary>
    public async Task LoginAndVerifyLink(string userName, string password)
    {
        await Login(userName, password);
        await Page.WaitForURLAsync("https://practicetestautomation.com/logged-in-successfully/");
    }
}