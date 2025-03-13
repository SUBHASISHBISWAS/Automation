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
// FileName: SpectrailHomePage.cs
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

public class SpectrailHomePage(IPage page, IPageFactory pageFactory) : BasePage(page)
{
    private readonly IPageFactory _pageFactory = pageFactory;

    private ILocator _aliviaLabel =>
        Page.GetByRole(AriaRole.Cell, new PageGetByRoleOptions { Name = "AVELIA project" });

    private ILocator _systemLabel => Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "System " });

    private ILocator _inputRequirementsLabel =>
        Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Input Requirements" });

    public async Task GoToAliviaProjectAndVerifyLink()
    {
        await Page.RunAndWaitForNavigationAsync(async () => { await _aliviaLabel.ClickAsync(); },
            new PageRunAndWaitForNavigationOptions
            {
                Timeout = 30000000,
                UrlString = "https://spectrail-dev.alstom.hub/spectrailvalid/2/Home/Index"
            });

        await GoInputRequirementsAndVerifyLink();
    }

    public async Task GoInputRequirementsAndVerifyLink()
    {
        await _systemLabel.ClickAsync();
        await Task.Delay(2000);
        await Page.RunAndWaitForNavigationAsync(async () =>
            {
                await _inputRequirementsLabel.ClickAsync();
                await Task.Delay(2000);
            },
            new PageRunAndWaitForNavigationOptions
            {
                Timeout = 30000000,
                UrlString = "https://spectrail-dev.alstom.hub/spectrailvalid/2/Requirement/Requirements"
            });
    }


    public async Task<bool> IsProjectVisible()
    {
        return await Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Projects" }).IsVisibleAsync();
    }
}