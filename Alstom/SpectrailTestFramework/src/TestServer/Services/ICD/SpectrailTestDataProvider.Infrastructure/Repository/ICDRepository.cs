#region

using Alstom.Spectrail.ICD.Application.Contracts;
using Alstom.Spectrail.ICD.Domain.Common;

#endregion

namespace SpectrailTestDataProvider.Infrastructure.Repository;

public class ICDRepository<T>(IDataProvider<T> dataProvider)
    : RepositoryBase<T>(dataProvider), IICDRepository<T> where T : EntityBase
{
}