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
// FileName: RegisterApplicationServices.cs
// ProjectName: Alstom.Spectrail.ICD.Application
// Created by SUBHASISH BISWAS On: 2025-03-21
// Updated by SUBHASISH BISWAS On: 2025-03-21
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.Application.Behaviours;
using Alstom.Spectrail.ICD.Application.Contracts;
using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Command;
using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Handlers;
using Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Handler;
using Alstom.Spectrail.ICD.Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using static System.Reflection.Assembly;

#endregion

namespace Alstom.Spectrail.ICD.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        // ReSharper disable once HeapView.ObjectAllocation

        services.AddAutoMapper(GetExecutingAssembly());
        services.AddValidatorsFromAssembly(GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RepositoryQueryHandler).Assembly));
        //services.RegisterRepositoryHandlers(typeof(DCUEntity).Assembly);
        services.AddScoped<IRequestHandler<SeedICDDataCommand, bool>, SeedICDDataCommandHandler>();
        services.AddScoped<IExcelService, ICDExcelService>();
        return services;
    }

    /*private static void RegisterRepositoryHandlers(this IServiceCollection services, Assembly assembly)
    {
        // ✅ Find all entity types inheriting from `EntityBase`
        var entityTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(EntityBase).IsAssignableFrom(t))
            .ToList();

        foreach (var entityType in entityTypes)
        {
            // ✅ Construct generic types dynamically
            var queryType = typeof(RepositoryQuery<>).MakeGenericType(entityType);
            var commandType = typeof(RepositoryCommand<>).MakeGenericType(entityType);
            var queryHandlerType = typeof(RepositoryQueryHandler<>).MakeGenericType(entityType);
            var commandHandlerType = typeof(RepositoryCommandHandler<>).MakeGenericType(entityType);
            var handlerInterfaceQuery =
                typeof(IRequestHandler<,>).MakeGenericType(queryType,
                    typeof(IEnumerable<>).MakeGenericType(entityType));
            var handlerInterfaceCommand = typeof(IRequestHandler<,>).MakeGenericType(commandType, typeof(bool));

            // ✅ Register query handler dynamically
            services.AddScoped(handlerInterfaceQuery, queryHandlerType);

            // ✅ Register command handler dynamically
            services.AddScoped(handlerInterfaceCommand, commandHandlerType);

            Console.WriteLine($"✅ Registered Repository Handlers for: {entityType.Name}");
        }
    }*/
}