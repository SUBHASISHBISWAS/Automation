#region

using Alstom.Spectrail.ICD.Application.Contracts;
using Alstom.Spectrail.ICD.Domain.Common;
using Alstom.Spectrail.Server.Common.Entities;
using Alstom.Spectrail.Server.Common.Repository;

#endregion

namespace Alstom.Spectrail.ICD.Infrastructure.Repository;

public class ICDRepository<T>(IDataProvider<T> dataProvider)
    : RepositoryBase<T>(dataProvider), IICDRepository<T> where T : EntityBase
{
}