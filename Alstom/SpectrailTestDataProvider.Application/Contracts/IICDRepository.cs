#region

using SpectrailTestDataProvider.Domain.Entities;

#endregion

namespace SpectrailTestDataProvider.Application.Contracts;

public interface IICDRepository : IAsyncRepository<ICDEntity>
{
}