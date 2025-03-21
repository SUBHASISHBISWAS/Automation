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
// FileName: EntityRegistrationMiddleware.cs
// ProjectName: Alstom.Spectrail.ICD.API
// Created by SUBHASISH BISWAS On: 2025-03-20
// Updated by SUBHASISH BISWAS On: 2025-03-21
//  ******************************************************************************/

#endregion

namespace Alstom.Spectrail.ICD.API.Middleware;

/// <summary>
///     ✅ Middleware that combines `EntityRegistrationMiddleware` and `ICDSeedDataMiddleware`.
///     ✅ Detects both **file changes** and **new entity additions**.
///     ✅ Ensures **MongoDB schema is up-to-date** without redundant executions.
///     ✅ Uses **Redis** for caching previous states.
/// </summary>
public class EntityRegistrationMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
{
    public async Task Invoke(HttpContext context)
    {
        using var scope = scopeFactory.CreateScope();
        var orchestrator = scope.ServiceProvider.GetRequiredService<EntityRegistryOrchestrator>();

        await orchestrator.ExecuteAsync(); // Runtime detection

        await next(context);
    }
}