#region

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SpectrailTestDataProvider.Application.Models;
using SpectrailTestDataProvider.Domain.Entities;

#endregion

namespace SpectrailTestDataProvider.Infrastructure.Persistence.Contexts.Mongo;

public class ICDMongoDataContext(
    IOptions<SpectrailMongoDatabaseSettings> databaseSettings,
    ILogger<ICDMongoDataContext> logger) : SpectrailMongoDbContext<ICDEntity>(databaseSettings, logger)
{
    public IMongoCollection<ICDEntity>? ICDData => SpectrailData;

    protected override IEnumerable<ICDEntity> GetPreconfiguredData()
    {
        throw new NotImplementedException();
    }
}