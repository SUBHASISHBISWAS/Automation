#region

using SpectrailTestDataProvider.Application.Contracts;
using SpectrailTestDataProvider.Domain.Common;
using SpectrailTestDataProvider.Infrastructure.Persistence.Drivers;

#endregion

namespace SpectrailTestDataProvider.Infrastructure.Repository;

public class ICDRepository<T>(MongoDataProvider<T> dataProvider)
    : RepositoryBase<T>(dataProvider), IICDRepository<T> where T : EntityBase
{
}