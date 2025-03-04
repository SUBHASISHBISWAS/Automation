#region

using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SpectrailTestDataProvider.Application.Models;
using SpectrailTestDataProvider.Domain.Common;

#endregion

namespace SpectrailTestDataProvider.Infrastructure.Persistence.Contexts.Mongo;

public class ICDMongoDataContext<T>(
    IOptions<SpectrailMongoDatabaseSettings> databaseSettings)
    : SpectrailMongoDbContext<T>(databaseSettings) where T : EntityBase, new()
{
    public IMongoCollection<T>? ICDData => SpectrailData;

    protected override IEnumerable<T> GetPreconfiguredData()
    {
        return new List<T>
        {
            new(),
            new(),
            new()
        };
    }
}