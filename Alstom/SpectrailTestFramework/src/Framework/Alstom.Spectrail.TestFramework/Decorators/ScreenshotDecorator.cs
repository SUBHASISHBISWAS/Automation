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
// FileName: ScreenshotDecorator.cs
// ProjectName: Alstom.Spectrail.TestFramework
// Created by SUBHASISH BISWAS On: 2025-03-13
// Updated by SUBHASISH BISWAS On: 2025-03-13
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.TestFramework.Interfaces;
using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Serilog;

#endregion

namespace Alstom.Spectrail.TestFramework.Decorators;

public class ScreenshotDecorator : BaseActionDecorator
{
    private static readonly string ScreenshotDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpectrailArtifacts", "Screenshots");

    private readonly string _testName = TestContext.CurrentContext.Test.Name;

    public ScreenshotDecorator(IActionHandler wrappedAction) : base(wrappedAction)
    {
        var testScreenshotDirectory = Path.Combine(ScreenshotDirectory, _testName);
        Directory.CreateDirectory(testScreenshotDirectory);

        // ✅ Register middleware at the time of instantiation
        Use(Middleware());
    }

    public static Func<IActionHandler, Func<Task>, Task> Middleware()
    {
        return async (handler, next) =>
        {
            try
            {
                await next();
                var page = handler.Page;
                if (page != null && TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Inconclusive)
                {
                    var screenshotPath = Path.Combine(ScreenshotDirectory, TestContext.CurrentContext.Test.Name,
                        "failure.png");
                    await page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                    Log.Information($"📸 Screenshot saved: {screenshotPath}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"❌ ScreenshotDecorator encountered an error: {ex.Message}");
                throw;
            }
        };
    }
}