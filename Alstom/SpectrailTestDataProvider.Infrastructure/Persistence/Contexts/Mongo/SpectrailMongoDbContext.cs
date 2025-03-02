#region

using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

#endregion

namespace SpectrailTestDataProvider.Infrastructure.Persistence.Contexts.Mongo;

public abstract class SpectrailMongoDbContext<T> : ISpectrailMongoDbContext<T> where T : class
{
    protected SpectrailMongoDbContext(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        //if (string.IsNullOrWhiteSpace(collectionName)) throw new ArgumentException("Collection name must be provided");
        var client = new MongoClient(configuration.GetConnectionString("DatabaseSettings:ConnectionString"));
        var database = client.GetDatabase(configuration.GetConnectionString("DatabaseSettings:DatabaseName"));
        //SpectrailData = database.GetCollection<T>(collectionName);
        SpectrailData = database.GetCollection<T>(typeof(T).Name);
        SeedDataAsync();
    }

    public void SeedDataAsync()
    {
        var isDataExist = SpectrailData.Find(p => true).Any();
        if (!isDataExist) SpectrailData?.InsertManyAsync(GetPreconfiguredData());
    }

    public IMongoCollection<T>? SpectrailData { get; set; }

    protected abstract IEnumerable<T> GetPreconfiguredData();
}