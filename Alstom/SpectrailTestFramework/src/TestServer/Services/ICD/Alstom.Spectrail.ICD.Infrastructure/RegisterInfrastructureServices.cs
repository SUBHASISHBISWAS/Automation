#region

using Alstom.Spectrail.ICD.Application.Contracts;
using Alstom.Spectrail.ICD.Application.Models;
using Alstom.Spectrail.ICD.Infrastructure.Persistence.Contexts.Mongo;
using Alstom.Spectrail.ICD.Infrastructure.Persistence.Drivers;
using Alstom.Spectrail.ICD.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace Alstom.Spectrail.ICD.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<SpectrailMongoDatabaseSettings>(options =>
            configuration.GetSection("SpectrailMongoDatabaseSettings").Bind(options));
        services.AddScoped(typeof(ISpectrailMongoDbContext<>), typeof(ICDMongoDataContext<>));
        services.AddScoped(typeof(IDataProvider<>), typeof(MongoDataProvider<>));
        services.AddScoped(typeof(IAsyncRepository<>), typeof(ICDRepository<>));
        services.AddScoped(typeof(IICDRepository<>), typeof(ICDRepository<>));

        return services;
    }
}