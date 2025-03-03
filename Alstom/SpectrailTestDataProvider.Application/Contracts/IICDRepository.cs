#region

using SpectrailTestDataProvider.Domain.Common;

#endregion

namespace SpectrailTestDataProvider.Application.Contracts;

public interface IICDRepository<T> : IAsyncRepository<T> where T : EntityBase
{
}