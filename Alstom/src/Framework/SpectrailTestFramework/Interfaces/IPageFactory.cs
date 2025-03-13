namespace SpectrailTestFramework.Interfaces;

public interface IPageFactory
{
    T CreatePage<T>() where T : IPageObject;
}