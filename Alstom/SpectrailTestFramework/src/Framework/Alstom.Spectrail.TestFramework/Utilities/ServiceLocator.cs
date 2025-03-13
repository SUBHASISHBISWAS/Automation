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
// FileName: ServiceLocator.cs
// ProjectName: Alstom.Spectrail.TestFramework
// Created by SUBHASISH BISWAS On: 2025-03-13
// Updated by SUBHASISH BISWAS On: 2025-03-13
//  ******************************************************************************/

#endregion

#region

using Microsoft.Extensions.DependencyInjection;

#endregion

namespace Alstom.Spectrail.TestFramework.Utilities;

public static class ServiceLocator
{
    private static IServiceProvider? _serviceProvider;

    public static void Initialize(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public static T GetService<T>() where T : notnull
    {
        return _serviceProvider == null
            ? throw new InvalidOperationException(
                "❌ ServiceProvider is not initialized. Ensure PlaywrightFactory.SetupDependencies() is called.")
            : _serviceProvider.GetRequiredService<T>();
    }

    public static IServiceProvider GetProvider()
    {
        return _serviceProvider ?? throw new InvalidOperationException("❌ ServiceProvider is not initialized.");
    }
}