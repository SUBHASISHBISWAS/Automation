#region

using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Alstom.Spectrail.ICD.Application.Contracts;
using Alstom.Spectrail.ICD.Application.Models;
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
    }

    private SpectrailMongoDatabaseSettings SpectrailDatabaseSettings { get; }

    public IMongoCollection<T>? SpectrailData { get; set; }
}