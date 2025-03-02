using SpectrailTestDataProvider.Domain.Common;

namespace SpectrailTestDataProvider.Application.Contracts;

public interface IDataContext<in T> where T : EntityBase
{
    void SeedDataAsync();
}