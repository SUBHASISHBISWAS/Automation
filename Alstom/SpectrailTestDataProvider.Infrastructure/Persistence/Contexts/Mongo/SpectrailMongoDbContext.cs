#region

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SpectrailTestDataProvider.Application.Models;
using SpectrailTestDataProvider.Domain.Common;

#endregion

namespace SpectrailTestDataProvider.Infrastructure.Persistence.Contexts.Mongo;

public abstract class SpectrailMongoDbContext<T> : ISpectrailMongoDbContext<T> where T : EntityBase
{
    protected SpectrailMongoDbContext(IOptions<SpectrailMongoDatabaseSettings> databaseSettings,
        ILogger<SpectrailMongoDbContext<T>> logger)
    {
        ArgumentNullException.ThrowIfNull(databaseSettings);
        SpectrailDatabaseSettings = databaseSettings.Value;
        Logger = logger;
        var client = new MongoClient(SpectrailDatabaseSettings.ConnectionString);
        var database = client.GetDatabase(SpectrailDatabaseSettings.DatabaseName);
        //SpectrailData = database.GetCollection<T>(collectionName);
        SpectrailData = database.GetCollection<T>(typeof(T).Name);
        SeedDataAsync();
    }

    private protected SpectrailMongoDatabaseSettings SpectrailDatabaseSettings { get; }

    private protected ILogger<SpectrailMongoDbContext<T>> Logger { get; }

    public void SeedDataAsync()
    {
        var isDataExist = SpectrailData.Find(p => true).Any();
        if (!isDataExist) SpectrailData?.InsertManyAsync(GetPreconfiguredData());
    }

    public IMongoCollection<T>? SpectrailData { get; set; }

    protected abstract IEnumerable<T> GetPreconfiguredData();
}