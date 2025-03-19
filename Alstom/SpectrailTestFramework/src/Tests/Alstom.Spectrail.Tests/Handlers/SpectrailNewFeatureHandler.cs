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
// FileName: SpectrailNewFeatureHandler.cs
// ProjectName: Alstom.Spectrail.Tests
// Created by SUBHASISH BISWAS On: 2025-03-13
// Updated by SUBHASISH BISWAS On: 2025-03-19
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.TestFramework.Actions;
using Alstom.Spectrail.TestFramework.API.Services;
using Alstom.Spectrail.TestFramework.Attributes;
using Alstom.Spectrail.TestFramework.Factory;
using Alstom.Spectrail.TestFramework.Interfaces;
using Alstom.Spectrail.Tests.Pages;

#endregion

namespace Alstom.Spectrail.Tests.Handlers;

[MapsToPage(typeof(SpectrailNewFeatureHandlerPage))]
public class SpectrailNewFeatureHandler(
    IPageObject pageObject,
    IHandlerFactory handlerFactory,
    ApiServiceFactory apiServiceFactory) : BaseActionHandler
{
    private readonly ApiServiceFactory _apiServiceFactory =
        apiServiceFactory ?? throw new ArgumentNullException(nameof(apiServiceFactory));

    private readonly IHandlerFactory _handlerFactory =
        handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));

    private readonly SpectrailNewFeatureHandlerPage _spectrailNewFeaturePage =
        pageObject as SpectrailNewFeatureHandlerPage ??
        throw new ArgumentException(
            "Invalid PageObject type.");

    private string _fileName = string.Empty;

    public SpectrailNewFeatureHandler WithFileName(string fileName)
    {
        ArgumentNullException.ThrowIfNull(fileName);
        _fileName = fileName;
        return this;
    }

    protected override async Task ExecuteAsync()
    {
        // ✅ Fetch Data from User Service
        var icdDataService = _apiServiceFactory.GetService("CustomColumnDataService") as CustomColumnDataService ??
                             throw new ArgumentNullException(nameof(CustomColumnDataService))
            ;
        var userResponse = await icdDataService.GetDCUAsync(_fileName);
    }
}