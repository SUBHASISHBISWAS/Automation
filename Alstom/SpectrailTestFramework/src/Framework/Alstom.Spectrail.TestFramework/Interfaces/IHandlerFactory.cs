namespace Alstom.Spectrail.TestFramework.Interfaces;

public interface IHandlerFactory
{
    T CreateHandler<T>() where T : IActionHandler;
}