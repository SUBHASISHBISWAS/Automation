#region

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpectrailTestDataProvider.Application.Contracts;
using SpectrailTestDataProvider.Infrastructure.Persistence;
using SpectrailTestDataProvider.Infrastructure.Persistence.Contexts.Mongo;
using SpectrailTestDataProvider.Infrastructure.Repository;

#endregion

namespace SpectrailTestDataProvider.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped(typeof(IDataContext<>), typeof(SpectrailMongoDbContext<>));
        services.AddScoped(typeof(ISpectrailMongoDbContext<>), typeof(ICDMongoDataContext));
        services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
        services.AddScoped<IICDRepository, ICDRepository>();
        return services;
    }
}