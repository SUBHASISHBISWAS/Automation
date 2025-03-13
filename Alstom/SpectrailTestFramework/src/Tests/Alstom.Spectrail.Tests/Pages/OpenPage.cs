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
// FileName: OpenPage.cs
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
///     ✅ **Handles generic page navigation in Playwright.**
///     ✅ **Supports navigation & waiting for page load.**
/// </summary>
public class OpenPage(IPage page, IPageFactory pageFactory) : BasePage(page)
{
    private readonly IPageFactory _pageFactory = pageFactory;


    /// <summary>
    ///     ✅ **Navigate to a URL**
    /// </summary>
    public async Task GoToUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) throw new ArgumentException("❌ URL must not be empty.", nameof(url));

        await Page.GotoAsync(url, new PageGotoOptions
        {
            Timeout = 30000000
        });
    }


    /// <summary>
    ///     ✅ **Wait until the page has fully loaded**
    /// </summary>
    public async Task WaitForPageLoad()
    {
        await Page.WaitForLoadStateAsync(LoadState.Load);
    }
}