namespace SpectrailTestFramework.Interfaces;

public interface IHandlerFactory
{
    T CreateHandler<T>() where T : IActionHandler;
}