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
// FileName: SpectrailTest.cs
// ProjectName: Alstom.Spectrail.Tests
// Created by SUBHASISH BISWAS On: 2025-02-19
// Updated by SUBHASISH BISWAS On: 2025-03-28
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.TestFramework.Utilities;
using Alstom.Spectrail.Tests.Handlers;
using Alstom.Spectrail.Tests.Hooks;
using NUnit.Allure.Attributes;
using NUnit.Framework;
using TestContext = NUnit.Framework.TestContext;

#endregion

namespace Alstom.Spectrail.Tests.Tests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class SpectrailTest : TestHooks
{
    [Test]
    [AllureFeature("Login")]
    public async Task Open_Spectrail_Goto_Project()
    {
        var testName = TestContext.CurrentContext.Test.Name;
        ExtentReportManager.StartTest(testName);
        ExtentReportManager.LogTestInfo("🚀 Starting Login Test...");

        var openPageHandler = ActionFactory?.GetAction<OpenPageHandler>()
            .WithUrl("https://spectrail-dev.alstom.hub/spectrailvalid/47/Home/Index");

        var spectrailPageHandler = ActionFactory?.GetAction<SpectrailPageHandler>();


        await openPageHandler?.SetNextAction(spectrailPageHandler).RunAsync();

        ExtentReportManager.LogTestPass("✅ Login Test Passed.");
        ExtentReportManager.FlushReport();
    }

    [Test]
    [AllureFeature("GET-ICDEntity")]
    public async Task GET_ICD_Entity()
    {
        var spectrailNewFeatureHandler = ActionFactory!.GetAction<SpectrailNewFeatureHandler>();
        await spectrailNewFeatureHandler.WithFileName("trdp_icd_generated").WithSheetName("dcu").RunAsync();
    }
}