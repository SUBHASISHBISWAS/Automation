#region

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpectrailTestDataProvider.Application.Contracts;
using SpectrailTestDataProvider.Application.Models;
using SpectrailTestDataProvider.Infrastructure.Persistence.Contexts.Mongo;
using SpectrailTestDataProvider.Infrastructure.Persistence.Drivers;
using SpectrailTestDataProvider.Infrastructure.Repository;

#endregion

namespace SpectrailTestDataProvider.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<SpectrailMongoDatabaseSettings>(options =>
            configuration.GetSection("SpectrailMongoDatabaseSettings").Bind(options));
        services.AddScoped(typeof(ISpectrailMongoDbContext<>), typeof(ICDMongoDataContext<>));
        services.AddScoped(typeof(IDataProvider<>), typeof(MongoDataProvider<>));
        services.AddScoped(typeof(IICDRepository<>), typeof(ICDRepository<>));
        return services;
    }
}