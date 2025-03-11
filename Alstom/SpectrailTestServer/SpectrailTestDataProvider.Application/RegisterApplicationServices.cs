#region

using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SpectrailTestDataProvider.Application.Behaviours;
using SpectrailTestDataProvider.Application.Contracts;
using SpectrailTestDataProvider.Application.Features.ICD.Queries.Handler;
using SpectrailTestDataProvider.Application.Features.ICD.Queries.Query;
using SpectrailTestDataProvider.Application.Features.ICD.Services;
using SpectrailTestDataProvider.Domain.Entities.ICD;

#endregion

namespace SpectrailTestDataProvider.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetRepositoryQueryHandler<>).Assembly));
        services.AddScoped<IRequestHandler<GetRepositoryQuery<DCUEntity>, IAsyncRepository<DCUEntity>>, GetRepositoryQueryHandler<DCUEntity>>();
        return services;
    }
}