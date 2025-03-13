#region

using SpectrailTestDataProvider.Domain.Common;

#endregion

namespace SpectrailTestDataProvider.Application.Contracts;

public interface IDataContext<in T> where T : EntityBase
{
}