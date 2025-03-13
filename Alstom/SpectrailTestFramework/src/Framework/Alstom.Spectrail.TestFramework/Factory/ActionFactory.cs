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
// FileName: ActionFactory.cs
// ProjectName: Alstom.Spectrail.TestFramework
// Created by SUBHASISH BISWAS On: 2025-03-13
// Updated by SUBHASISH BISWAS On: 2025-03-13
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.TestFramework.Actions;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

#endregion

namespace Alstom.Spectrail.TestFramework.Factory;

public class ActionFactory(IServiceProvider serviceProvider)
{
    private readonly IServiceProvider _serviceProvider =
        serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    /// <summary>
    ///     ✅ **Creates and returns an instance of the requested handler with decorators applied.**
    /// </summary>
    public T GetAction<T>() where T : BaseActionHandler
    {
        try
        {
            // ✅ Retrieve the base handler instance from DI
            var actionInstance = _serviceProvider.GetRequiredService<T>();

            // ✅ Ensure correct return type
            return actionInstance ?? throw new InvalidOperationException(
                $"❌ Decorator wrapping error: {actionInstance?.GetType().Name} cannot be cast to {typeof(T).Name}");
        }
        catch (Exception ex)
        {
            Log.Error($"[ActionFactory] ❌ Failed to create instance of {typeof(T).Name}: {ex.Message}");
            throw new InvalidOperationException($"Unable to create instance of {typeof(T).Name}.", ex);
        }
    }
}