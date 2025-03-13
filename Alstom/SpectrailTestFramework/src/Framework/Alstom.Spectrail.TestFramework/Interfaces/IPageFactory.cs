namespace Alstom.Spectrail.TestFramework.Interfaces;

public interface IPageFactory
{
    T CreatePage<T>() where T : IPageObject;
}