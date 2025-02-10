using System;
using System.Threading.Tasks;
namespace Alstom.Spectrail.Framework.Actions
{
    public abstract class BaseActionHandler : IActionHandler
    {
        private IActionHandler _nextHandler;
        public Func<Task> DelayFunction { get; set; } = null;
        public void SetNext(IActionHandler nextHandler)
        {
            _nextHandler = nextHandler;
        }
        public async Task HandleAsync()
        {
            if (DelayFunction != null)
            {
                await DelayFunction.Invoke();
            }
            await ExecuteAsync();
            if (_nextHandler != null)
            {
                await _nextHandler.HandleAsync();
            }
        }
        protected abstract Task ExecuteAsync();
    }
}