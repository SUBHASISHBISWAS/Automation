#region

using SpectrailTestDataProvider.Application.Contracts;
using SpectrailTestDataProvider.Domain.Entities;
using SpectrailTestDataProvider.Infrastructure.Persistence.Drivers;

#endregion

namespace SpectrailTestDataProvider.Infrastructure.Repository;

public class ICDRepository(MongoDataProvider<ICDEntity> dataProvider)
    : RepositoryBase<ICDEntity>(dataProvider), IICDRepository
{
}