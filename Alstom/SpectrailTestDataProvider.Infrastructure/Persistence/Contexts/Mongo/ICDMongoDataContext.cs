#region

using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using SpectrailTestDataProvider.Domain.Entities;

#endregion

namespace SpectrailTestDataProvider.Infrastructure.Persistence.Contexts.Mongo;

public class ICDMongoDataContext(IConfiguration configuration) : SpectrailMongoDbContext<ICDEntity>(configuration)
{
    public IMongoCollection<ICDEntity>? ICDData => SpectrailData;

    protected override IEnumerable<ICDEntity> GetPreconfiguredData()
    {
        throw new NotImplementedException();
    }
}