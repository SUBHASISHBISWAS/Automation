#region

using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SpectrailTestDataProvider.Application.Contracts;
using SpectrailTestDataProvider.Application.Models;
using SpectrailTestDataProvider.Domain.Common;

#endregion

namespace SpectrailTestDataProvider.Infrastructure.Persistence.Contexts.Mongo;

public abstract class SpectrailMongoDbContext<T> : ISpectrailMongoDbContext<T> where T : EntityBase
{
    protected SpectrailMongoDbContext(IOptions<SpectrailMongoDatabaseSettings> databaseSettings)
    {
        ArgumentNullException.ThrowIfNull(databaseSettings);
        SpectrailDatabaseSettings = databaseSettings.Value;
        var client = new MongoClient(SpectrailDatabaseSettings.ConnectionString);
        var database = client.GetDatabase(SpectrailDatabaseSettings.DatabaseName);
        SpectrailData = database.GetCollection<T>(typeof(T).Name);
        SeedDataAsync();
    }

    private SpectrailMongoDatabaseSettings SpectrailDatabaseSettings { get; }

    public IMongoCollection<T>? SpectrailData { get; set; }


    public void SeedDataAsync()
    {
        var isDataExist = SpectrailData.Find(p => true).Any();
        if (!isDataExist) SpectrailData?.InsertManyAsync(GetPreconfiguredData());
    }

    protected abstract IEnumerable<T> GetPreconfiguredData();
}