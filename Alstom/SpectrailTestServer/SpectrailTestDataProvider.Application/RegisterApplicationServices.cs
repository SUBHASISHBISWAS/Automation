#region

using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SpectrailTestDataProvider.Application.Behaviours;
using SpectrailTestDataProvider.Application.Contracts;
using SpectrailTestDataProvider.Application.Features.ICD.Queries.Handler;
using SpectrailTestDataProvider.Application.Features.ICD.Queries.Query;
using SpectrailTestDataProvider.Application.Utility;
using SpectrailTestDataProvider.Domain.Entities.ICD;
using static System.Reflection.Assembly;

#endregion

namespace SpectrailTestDataProvider.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        // ReSharper disable once HeapView.ObjectAllocation
        ApplicationConfigHelper configHelper = new();
        services.AddSingleton(configHelper);
        services.AddAutoMapper(GetExecutingAssembly());
        services.AddValidatorsFromAssembly(GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetRepositoryQueryHandler<>).Assembly));
        services
            .AddScoped<IRequestHandler<GetRepositoryQuery<DCUEntity>, IAsyncRepository<DCUEntity>>,
                GetRepositoryQueryHandler<DCUEntity>>();
        return services;
    }
}