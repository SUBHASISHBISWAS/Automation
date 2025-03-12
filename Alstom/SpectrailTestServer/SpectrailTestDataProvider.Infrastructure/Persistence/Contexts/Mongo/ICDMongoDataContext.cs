#region

using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SpectrailTestDataProvider.Application.Models;
using SpectrailTestDataProvider.Domain.Common;
using SpectrailTestDataProvider.Domain.Entities.ICD;

#endregion

namespace SpectrailTestDataProvider.Infrastructure.Persistence.Contexts.Mongo;

public class ICDMongoDataContext<T>(
    IOptions<SpectrailMongoDatabaseSettings> databaseSettings)
    : SpectrailMongoDbContext<DCUEntity>(databaseSettings) where T : EntityBase
{
    public IMongoCollection<DCUEntity>? ICDData => SpectrailData;
}