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
// Updated by SUBHASISH BISWAS On: 2025-03-22
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Command;
using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Handlers;
using Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Handler;
using Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Query;
using Alstom.Spectrail.Server.Common.Entities;
using Autofac;
using MediatR;

#endregion

namespace Alstom.Spectrail.ICD.Application.Utility;

public static class DynamicRepositoryRegistrar
{
    public static void RegisterRepositoryHandlers(ContainerBuilder builder, IEnumerable<Type> dynamicTypes)
    {
        // ✅ Register shared non-generic query handler
        builder
            .RegisterType<RepositoryQueryHandler>()
            .As<IRequestHandler<RepositoryQuery, IEnumerable<EntityBase>>>()
            .InstancePerLifetimeScope();

        foreach (var entityType in dynamicTypes)
            try
            {
                var commandType = typeof(RepositoryCommand);
                var commandHandlerType = typeof(RepositoryCommandHandler);
                var commandInterface = typeof(IRequestHandler<,>).MakeGenericType(commandType, typeof(bool));

                builder
                    .RegisterType(commandHandlerType)
                    .As(commandInterface)
                    .InstancePerLifetimeScope();

                Console.WriteLine($"✅ Registered dynamic command handler for: {entityType.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to register dynamic handler for {entityType.Name}: {ex.Message}");
            }
    }
}