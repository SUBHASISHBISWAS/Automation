#region

using Microsoft.Extensions.Options;
using SpectrailTestDataProvider.Application.Models;
using SpectrailTestDataProvider.Domain.Common;

#endregion

namespace SpectrailTestDataProvider.Infrastructure.Persistence.Contexts.Mongo;

public class ICDMongoDataContext<T>(
    IOptions<SpectrailMongoDatabaseSettings> databaseSettings)
    : SpectrailMongoDbContext<T>(databaseSettings) where T : EntityBase
{
    //public IMongoCollection<ICDEntity>? ICDData => SpectrailData;

    protected override IEnumerable<T> GetPreconfiguredData()
    {
        throw new NotImplementedException();
    }
}