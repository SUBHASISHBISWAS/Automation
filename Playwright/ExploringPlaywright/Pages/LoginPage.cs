using System.ComponentModel.DataAnnotations;
using Microsoft.Playwright;
namespace ExploringPlaywright.Pages;


public class LoginPage
{
    private readonly IPage _page;
    
    public LoginPage(IPage page)=> _page = page;
    
    private ILocator _userName=> _page.GetByLabel("Username");
    private ILocator _password=> _page.GetByLabel("Password");
    private ILocator _submitButton=> _page.GetByRole(AriaRole.Button, new() { Name = "Submit" });
    private ILocator _loggedInSuccessfully=> _page.GetByRole(AriaRole.Heading, new() { Name = "Logged In Successfully" });

    public async Task Login(string userName, string password)
    {
        await _userName.FillAsync(userName);
        await _password.FillAsync(password);
        await _submitButton.ClickAsync();
    }
    public async Task LoginAndVerifyLink(string userName, string password)
    {
        await _userName.FillAsync(userName);
        await _password.FillAsync(password);
        await _page.RunAndWaitForNavigationAsync(async () => { await _submitButton.ClickAsync(); },
            new PageRunAndWaitForNavigationOptions()
            {
                UrlString = "https://practicetestautomation.com/logged-in-successfully/"
            });

    }

    public async Task<bool> IsLoggedIn() => await _loggedInSuccessfully.IsVisibleAsync();
}


