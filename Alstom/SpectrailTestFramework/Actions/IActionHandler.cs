namespace Alstom.Spectrail.Framework.Actions
{
    public interface IActionHandler
    {
        Task HandleAsync();
        void SetNext(IActionHandler nextHandler);
        Func<Task> DelayFunction { get; set; }
    }
}