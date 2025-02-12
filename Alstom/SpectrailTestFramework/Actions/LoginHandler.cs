using System;
using System.Threading.Tasks;

using Microsoft.Playwright;

using SpectrailTestFramework.Decorators;
using SpectrailTestFramework.Factory;
using SpectrailTestFramework.PageObjects;

namespace SpectrailTestFramework.Actions;

/// <summary>
/// ✅ **Handles login action logic.**  
/// ✅ **Ensures user input validation.**  
/// ✅ **Applies middleware decorators dynamically.**  
/// </summary>
public class LoginHandler : BaseActionHandler
{
    private readonly LoginPage _loginPage;
    private string _password = string.Empty;
    private string _username = string.Empty;
    private bool _verifyLink;

    /// <summary>
    /// ✅ **Constructor for LoginHandler.**
    /// </summary>
    public LoginHandler(ActionFactory actionFactory) : base(actionFactory)
    {
        _loginPage = _actionFactory.CreatePage<LoginPage>(); // ✅ Retrieves `LoginPage` instance
    }

    /// ✅ **Expose Playwright Page to allow decorators to access it**
    public override IPage? Page => _loginPage.Page;

    /// <summary>
    /// ✅ **Sets the username (Fluent API).**
    /// </summary>
    public LoginHandler WithUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("❌ Username must not be empty.", nameof(username));
        }

        _username = username;
        return this;
    }

    /// <summary>
    /// ✅ **Sets the password (Fluent API).**
    /// </summary>
    public LoginHandler WithPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("❌ Password must not be empty.", nameof(password));
        }

        _password = password;
        return this;
    }

    /// <summary>
    /// ✅ **Toggles verification of navigation after login.**
    /// </summary>
    public LoginHandler VerifyNavigation(bool verify = true)
    {
        _verifyLink = verify;
        return this;
    }

    /// <summary>
    /// ✅ **Executes the login process.**
    /// </summary>
    protected override async Task ExecuteAsync()
    {
        if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
        {
            throw new InvalidOperationException("❌ Username and Password must be set before execution.");
        }

        if (_verifyLink)
        {
            await LoginAndVerifyLink();
        }
        else
        {
            await Login();
        }
    }

    /// <summary>
    /// ✅ **Handles simple login action.**
    /// </summary>
    private async Task Login()
    {
        await _loginPage.Login(_username, _password);
    }

    /// <summary>
    /// ✅ **Handles login and ensures post-login navigation.**
    /// </summary>
    private async Task LoginAndVerifyLink()
    {
        await _loginPage.LoginAndVerifyLink(_username, _password);
    }
}