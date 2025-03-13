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
// FileName: SpectrailNewFeatureHandlerPage.cs
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

public class SpectrailNewFeatureHandlerPage(IPage page, IPageFactory pageFactory) : BasePage(page)
{
    private readonly IPageFactory _pageFactory = pageFactory;
    private IPage _page = page;


    public async Task CheckDuplicate()
    {
        _page = Page;
        await _page.GotoAsync(Config.GetUrl("SpectrailValid"));
        await _page.GetByRole(AriaRole.Cell, new PageGetByRoleOptions { Name = "AVELIA project" }).ClickAsync();
        await _page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Architecture " }).ClickAsync();
        await _page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Devices" }).ClickAsync();
        await _page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = " Instantiations" }).ClickAsync();
        await _page.Locator("#ListInstanciation_DXFREditorcol0").GetByRole(AriaRole.Cell).ClickAsync();
        await _page.Locator("#ListInstanciation_DXFREditorcol0_I").FillAsync("Axlei");
        await _page.Locator("#ListInstanciation_DXFREditorcol0_I").PressAsync("Enter");
        await _page.GetByRole(AriaRole.Cell, new PageGetByRoleOptions { Name = "<Axlei>", Exact = true }).ClickAsync();
        await _page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Enter substitution (one value" })
            .ClickAsync();
        await _page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Enter substitution (one value" })
            .FillAsync("1\n2\nAxle1\n2");
        await _page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Update" }).ClickAsync();
        await _page.GetByText("Following are duplicate").ClickAsync();
        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Cancel" }).ClickAsync();
    }
}