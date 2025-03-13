#region ©COPYRIGHT

// /*******************************************************************************
//  *   © COPYRIGHT ALSTOM SA 2025 - ALL RIGHTS RESERVED
//  *
//  *  This file is part of the Alstom Spectrail project.
//  *  Unauthorized copying, modification, distribution, or use of this file,
//  *  via any medium, is strictly prohibited.
//  *
//  *  Proprietary and confidential.
//  *  Created and maintained by Alstom for internal use in the Spectrail project.
//  ******************************************************************************/
// 
//  /*******************************************************************************
// AuthorName: SUBHASISH BISWAS
// Email: subhasish.biswas@alstomgroup.com
// FileName: LoginPage.cs
// ProjectName: Alstom.Spectrail.Tests
// Created by SUBHASISH BISWAS On: 2025-03-13
// Updated by SUBHASISH BISWAS On: 2025-03-13
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.TestFramework.Interfaces;
using Alstom.Spectrail.TestFramework.PageObjects;
using Microsoft.Playwright;

#endregion

namespace Alstom.Spectrail.Tests.Pages;

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