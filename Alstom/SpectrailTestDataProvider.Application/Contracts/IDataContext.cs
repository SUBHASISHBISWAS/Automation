namespace SpectrailTestDataProvider.Application.Contracts;

public interface IDataContext<in T> where T : class
{
    void SeedDataAsync();
}