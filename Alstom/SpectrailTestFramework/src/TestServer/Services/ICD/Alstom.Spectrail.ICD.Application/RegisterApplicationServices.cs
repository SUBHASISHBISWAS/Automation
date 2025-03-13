#region

using Alstom.Spectrail.ICD.Application.Behaviours;
using Alstom.Spectrail.ICD.Application.Contracts;
using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Command;
using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Handlers;
using Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Handler;
using Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Query;
using Alstom.Spectrail.ICD.Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Alstom.Spectrail.ICD.Domain.Entities.ICD;
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

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RepositoryQueryHandler<>).Assembly));
        services
            .AddScoped<IRequestHandler<RepositoryQuery<DCUEntity>, IEnumerable<DCUEntity>>,
                RepositoryQueryHandler<DCUEntity>>();

        services
            .AddScoped<IRequestHandler<RepositoryCommand<DCUEntity>, bool>,
                RepositoryCommandHandler<DCUEntity>>();
        services.AddScoped<IRequestHandler<SeedICDDataCommand, bool>, SeedICDDataCommandHandler>();
        services.AddScoped<IExcelService, ICDExcelService>();
        return services;
    }
}