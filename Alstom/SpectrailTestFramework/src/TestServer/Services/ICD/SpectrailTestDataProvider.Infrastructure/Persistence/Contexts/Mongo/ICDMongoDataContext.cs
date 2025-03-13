#region

using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Alstom.Spectrail.ICD.Application.Models;
using Alstom.Spectrail.ICD.Domain.Common;
using Alstom.Spectrail.ICD.Domain.Entities.ICD;

#endregion

namespace SpectrailTestDataProvider.Infrastructure.Persistence.Contexts.Mongo;

public class ICDMongoDataContext<T>(
    IOptions<SpectrailMongoDatabaseSettings> databaseSettings)
    : SpectrailMongoDbContext<DCUEntity>(databaseSettings) where T : EntityBase
{
    public IMongoCollection<DCUEntity>? ICDData => SpectrailData;
}