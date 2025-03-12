#region

using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SpectrailTestDataProvider.Application.Behaviours;
using SpectrailTestDataProvider.Application.Features.ICD.Commands.Command;
using SpectrailTestDataProvider.Application.Features.ICD.Commands.Handlers;
using SpectrailTestDataProvider.Application.Features.ICD.Queries.Handler;
using SpectrailTestDataProvider.Application.Features.ICD.Queries.Query;
using SpectrailTestDataProvider.Domain.Entities.ICD;
using static System.Reflection.Assembly;

#endregion

namespace SpectrailTestDataProvider.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        // ReSharper disable once HeapView.ObjectAllocation

        services.AddAutoMapper(GetExecutingAssembly());
        services.AddValidatorsFromAssembly(GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        // ✅ Automatically register all MediatR handlers from the assembly
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RepositoryQueryHandler<>).Assembly));
        services
            .AddScoped<IRequestHandler<RepositoryQuery<DCUEntity>, IEnumerable<DCUEntity>>,
                RepositoryQueryHandler<DCUEntity>>();

        services
            .AddScoped<IRequestHandler<RepositoryCommand<DCUEntity>, bool>,
                RepositoryCommandHandler<DCUEntity>>();


        /*services.Scan(scan => scan
            .FromAssemblies(typeof(RepositoryCommandHandler<>).Assembly) // ✅ Scan the correct assembly
            .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<,>))) // ✅ Find all MediatR handlers
            .AsImplementedInterfaces() // ✅ Register as implemented interfaces
            .WithScopedLifetime()); // ✅ Use scoped lifetime (same as before)

        services.Scan(scan => scan
            .FromAssemblies(typeof(RepositoryQueryHandler<>).Assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());*/


        /*
        services
            .AddScoped<IRequestHandler<GetRepositoryQuery<DCUEntity>, IAsyncRepository<DCUEntity>>,
                GetRepositoryQueryHandler<DCUEntity>>();
                */
        return services;
    }
}