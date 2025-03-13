#region

using SpectrailTestDataProvider.Domain.Common;

#endregion

namespace Alstom.Spectrail.ICD.Application.Contracts;

public interface IICDRepository<T> : IAsyncRepository<T> where T : EntityBase
{
}