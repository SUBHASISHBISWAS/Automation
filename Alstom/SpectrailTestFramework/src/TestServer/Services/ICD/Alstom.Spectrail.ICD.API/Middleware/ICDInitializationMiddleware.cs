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
// FileName: ICDInitializationMiddleware.cs
// ProjectName: Alstom.Spectrail.ICD.API
// Created by SUBHASISH BISWAS On: 2025-03-12
// Updated by SUBHASISH BISWAS On: 2025-03-17
//  ******************************************************************************/

#endregion

#region

using System.Diagnostics;
using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Command;
using Alstom.Spectrail.ICD.Application.Registry;
using Alstom.Spectrail.ICD.Infrastructure.Persistence.Contexts.Mongo;
using Alstom.Spectrail.Server.Common.Configuration;
using MediatR;

#endregion

namespace Alstom.Spectrail.ICD.API.Middleware;

public class ICDSeedDataMiddleware(
    RequestDelegate next,
    IServiceScopeFactory scopeFactory,
    IServerConfigHelper configHelper)
{
    private static bool _initialized;

    public async Task Invoke(HttpContext context)
    {
        if (!_initialized && configHelper.IsFeatureEnabled("EnableMiddlewarePreloading"))
        {
            Debug.WriteLine("🚀 Middleware Preloading Enabled: Initializing MongoDB & Registering Entities...");

            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ICDMongoDataContext>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            try
            {
                // ✅ Step 1: Ensure MongoDB is initialized
                Debug.WriteLine("🔍 Ensuring MongoDB collections exist...");
                var entityRegistry = scope.ServiceProvider.GetRequiredService<EntityRegistry>();
                entityRegistry.RegisterEntity();
                Debug.WriteLine("✅ Entities are Register Now!");

                // ✅ Step 2: Send Seed Command
                var success = await mediator.Send(new SeedICDDataCommand());

                if (success)
                {
                    _initialized = true;
                    Debug.WriteLine("✅ ICD Data Seeding Completed via Middleware!");
                }
                else
                {
                    Debug.WriteLine("⚠️ ICD Data Seeding Failed via Middleware!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error in Middleware: {ex.Message}");
            }
        }

        await next(context);
    }
}