using Microsoft.Playwright;

using SpectrailTestFramework.Attributes;
using SpectrailTestFramework.Factory;
using SpectrailTestFramework.PageObjects;

namespace SpectrailTestFramework.Actions;

[ApplyLogging]
[ApplyScreenshot]
[ApplyVideo] // ✅ Automatically enables video recording for failures
public class LoginHandler : BaseActionHandler
{
    private readonly LoginPage _loginPage;
    private string _password = string.Empty;
    private string _username = string.Empty;
    private bool _verifyLink;

    /// <summary>
    ///     Constructor for LoginHandler.
    /// </summary>
    public LoginHandler(ActionFactory actionFactory) : base(actionFactory)
    {
        _loginPage = _actionFactory.CreatePage<LoginPage>(); // ✅ Automatically retrieves LoginPage
    }

    public override IPage? Page => _loginPage.Page; // ✅ Now properly exposes Playwright Page

    public LoginHandler WithUsername(string username)
    {
        _username = username;
        return this;
    }

    public LoginHandler WithPassword(string password)
    {
        _password = password;
        return this;
    }

    public LoginHandler VerifyNavigation(bool verify = true)
    {
        _verifyLink = verify;
        return this;
    }

    protected override async Task ExecuteAsync()
    {
        await LoginAndVerifyLink();
    }

    
    private async Task<LoginHandler?> Login()
    {
        if (!await _loginPage.IsSubmitButtonVisible()) return null;
        await _loginPage.Login(_username, _password);
        return this;

    }

    private async Task<LoginHandler?> LoginAndVerifyLink()
    {
        if (!await _loginPage.IsSubmitButtonVisible()) return null;
        await _loginPage.LoginAndVerifyLink(_username, _password);
        return this;
    }
}