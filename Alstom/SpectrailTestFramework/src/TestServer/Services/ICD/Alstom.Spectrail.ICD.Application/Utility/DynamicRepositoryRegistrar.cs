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
// FileName: DynamicRepositoryRegistrar.cs
// ProjectName: Alstom.Spectrail.ICD.Application
// Created by SUBHASISH BISWAS On: 2025-03-21
// Updated by SUBHASISH BISWAS On: 2025-03-21
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Command;
using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Handlers;
using Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Handler;
using Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Query;
using Alstom.Spectrail.Server.Common.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace Alstom.Spectrail.ICD.Application.Utility;

public static class DynamicRepositoryRegistrar
{
    public static void RegisterRepositoryHandlers(IServiceCollection services, IEnumerable<Type> dynamicTypes)
    {
        // ✅ Register the non-generic RepositoryQueryHandler once
        services.AddScoped<IRequestHandler<RepositoryQuery, IEnumerable<EntityBase>>, RepositoryQueryHandler>();

        foreach (var entityType in dynamicTypes)
            try
            {
                // ✅ Register RepositoryCommandHandler<T> for each dynamic entity
                var commandType = typeof(RepositoryCommand<>).MakeGenericType(entityType);
                var commandHandlerType = typeof(RepositoryCommandHandler<>).MakeGenericType(entityType);
                var commandInterface = typeof(IRequestHandler<,>).MakeGenericType(commandType, typeof(bool));

                services.AddScoped(commandInterface, commandHandlerType);

                Console.WriteLine($"✅ Registered Repository Command Handler for: {entityType.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to register handlers for {entityType.Name}: {ex.Message}");
                throw;
            }
    }
}