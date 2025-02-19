using SpectrailTestFramework.Actions;
using SpectrailTestFramework.Attributes;
using SpectrailTestFramework.Interfaces;

using SpectrailTests.Pages;

namespace SpectrailTests.Handlers;

[MapsToPage(typeof(LoginPage))] // ✅ Maps to `LoginPage`
public class LoginHandler : BaseActionHandler
{
    private readonly IHandlerFactory _handlerFactory;
    private readonly LoginPage _loginPage;
    private string _password = string.Empty;
    private string _username = string.Empty;

    public LoginHandler(IPageObject pageObject, IHandlerFactory handlerFactory)
    {
        _loginPage = pageObject as LoginPage ?? throw new ArgumentException("Invalid PageObject type.");
        _handlerFactory = handlerFactory;
    }

    public LoginHandler WithUsername(string username)
    {
        _username = username ?? throw new ArgumentNullException(nameof(username));
        return this;
    }

    public LoginHandler WithPassword(string password)
    {
        _password = password ?? throw new ArgumentNullException(nameof(password));
        return this;
    }

    protected override async Task ExecuteAsync()
    {
        if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
        {
            throw new InvalidOperationException("❌ Username and Password must be set before execution.");
        }

        await _loginPage.Login(_username, _password);
    }
}