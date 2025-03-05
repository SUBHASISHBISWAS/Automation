#region

using SpectrailTestFramework.Actions;
using SpectrailTestFramework.Attributes;
using SpectrailTestFramework.Factory;
using SpectrailTestFramework.Interfaces;
using SpectrailTests.Pages;

#endregion

namespace SpectrailTests.Handlers;

[MapsToPage(typeof(LoginPage))] // ✅ Maps to `LoginPage`
public class LoginHandler(IPageObject pageObject, IHandlerFactory handlerFactory, ApiServiceFactory apiServiceFactory)
    : BaseActionHandler
{
    private readonly ApiServiceFactory _apiServiceFactory =
        apiServiceFactory ?? throw new ArgumentNullException(nameof(apiServiceFactory));

    private readonly IHandlerFactory _handlerFactory =
        handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));

    private readonly LoginPage _loginPage =
        pageObject as LoginPage ?? throw new ArgumentException("Invalid PageObject type.");

    private string _password = string.Empty;
    private string _username = string.Empty;

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
            throw new InvalidOperationException("❌ Username and Password must be set before execution.");

        await _loginPage.Login(_username, _password);
    }
}