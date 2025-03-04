#region

using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SpectrailTestDataProvider.Application.Models;
using SpectrailTestDataProvider.Domain.Common;
using SpectrailTestDataProvider.Domain.Entities;

#endregion

namespace SpectrailTestDataProvider.Infrastructure.Persistence.Contexts.Mongo;

public class ICDMongoDataContext<T>(
    IOptions<SpectrailMongoDatabaseSettings> databaseSettings)
    : SpectrailMongoDbContext<ICDEntity>(databaseSettings) where T : EntityBase
{
    public IMongoCollection<ICDEntity>? ICDData => SpectrailData;

    protected override IEnumerable<ICDEntity> GetPreconfiguredData()
    {
        return new List<ICDEntity>
        {
            new()
            {
                CreatedDate = DateTime.Now,
                CreatedBy = "System",
                LastModifiedDate = DateTime.Now,
                LastModifiedBy = "System",
                ICDName = "Hello",
                ICDDescription = "HelloDescription"
            },
            new()
            {
                CreatedDate = DateTime.Now,
                CreatedBy = "SUBHASISH",
                LastModifiedDate = DateTime.Now,
                LastModifiedBy = "SUBHASISH",
                ICDName = "HI",
                ICDDescription = "HI Description"
            }
        };
    }
}