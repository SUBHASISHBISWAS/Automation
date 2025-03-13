#region

using Alstom.Spectrail.ICD.Application.Models;
using Alstom.Spectrail.ICD.Domain.Entities.ICD;
using Alstom.Spectrail.Server.Common.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

#endregion

namespace Alstom.Spectrail.ICD.Infrastructure.Persistence.Contexts.Mongo;

public class ICDMongoDataContext<T>(
    IOptions<SpectrailMongoDatabaseSettings> databaseSettings)
    : SpectrailMongoDbContext<DCUEntity>(databaseSettings) where T : EntityBase
{
    public IMongoCollection<DCUEntity>? ICDData => SpectrailData;
}