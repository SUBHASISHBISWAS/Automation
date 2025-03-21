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
// FileName: DynamicRepositoryAutofacModule.cs
// ProjectName: Alstom.Spectrail.ICD.API
// Created by SUBHASISH BISWAS On: 2025-03-21
// Updated by SUBHASISH BISWAS On: 2025-03-21
//  ******************************************************************************/

#endregion

/*#region ©COPYRIGHT

// /*******************************************************************************
//  *   © COPYRIGHT ALSTOM SA 2025 - ALL RIGHTS RESERVED
//  *
//  *  This file is part of the Alstom Spectrail project.
//  *  Unauthorized copying, modification, distribution, or use of this file,
//  *  via any medium, is strictly prohibited.
//  *
//  *  Proprietary and confidential.
//  *  Created and maintained by Alstom for internal use in the Spectrail project.
//  *****************************************************************************#1#
//
//  /*******************************************************************************
// AuthorName: SUBHASISH BISWAS
// Email: subhasish.biswas@alstomgroup.com
// FileName: DynamicRepositoryAutofacModule.cs
// ProjectName: Alstom.Spectrail.ICD.API
// Created by SUBHASISH BISWAS On: 2025-03-21
// Updated by SUBHASISH BISWAS On: 2025-03-21
//  *****************************************************************************#1#

#endregion

#region

using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Command;
using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Handlers;
using Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Handler;
using Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Query;
using Alstom.Spectrail.ICD.Application.Registry;
using Autofac;
using MediatR;

#endregion

public class DynamicRepositoryAutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var dynamicTypes = EntityRegistry.GetAllEntityTypes(); // Your dynamic entity types

        foreach (var entityType in dynamicTypes)
        {
            var queryType = typeof(RepositoryQuery).MakeGenericType(entityType);
            var handlerType = typeof(RepositoryQueryHandler).MakeGenericType(entityType);
            var handlerInterface =
                typeof(IRequestHandler<,>).MakeGenericType(queryType,
                    typeof(IEnumerable<>).MakeGenericType(entityType));

            builder.RegisterType(handlerType).As(handlerInterface).InstancePerLifetimeScope();

            // Register the command handler if needed
            var commandType = typeof(RepositoryCommand<>).MakeGenericType(entityType);
            var commandHandlerType = typeof(RepositoryCommandHandler<>).MakeGenericType(entityType);
            var commandInterface = typeof(IRequestHandler<,>).MakeGenericType(commandType, typeof(bool));

            builder.RegisterType(commandHandlerType).As(commandInterface).InstancePerLifetimeScope();

            Console.WriteLine($"✅ Autofac Registered: {entityType.Name}");
        }
    }
}*/
