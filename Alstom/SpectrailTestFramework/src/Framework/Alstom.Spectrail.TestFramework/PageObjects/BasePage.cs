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
// FileName: BasePage.cs
// ProjectName: Alstom.Spectrail.TestFramework
// Created by SUBHASISH BISWAS On: 2025-03-13
// Updated by SUBHASISH BISWAS On: 2025-03-13
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.TestFramework.Interfaces;
using Alstom.Spectrail.TestFramework.Utilities;
using Microsoft.Playwright;

#endregion

namespace Alstom.Spectrail.TestFramework.PageObjects;

/// <summary>
///     ✅ **Base class for all Playwright Page Objects.**
///     ✅ **Ensures each page object has access to Playwright's `IPage`.**
///     ✅ **Provides common navigation and wait methods.**
/// </summary>
public abstract class BasePage : IPageObject
{
    /// ✅ **Constructor now takes `IPage` directly (avoids circular DI issue)**
    protected BasePage(IPage page)
    {
        Page = page ?? throw new ArgumentNullException(nameof(page));
    }

    /// <summary>
    ///     ✅ **Predefined Property to Get ConfigHelper Easily**
    /// </summary>
    protected ConfigHelper Config => GetService<ConfigHelper>();

    public IPage Page { get; }

    /// <summary>
    ///     ✅ **Generic Property for Resolving Any Service from ServiceLocator**
    /// </summary>
    protected T GetService<T>() where T : notnull
    {
        return ServiceLocator.GetService<T>();
    }
}