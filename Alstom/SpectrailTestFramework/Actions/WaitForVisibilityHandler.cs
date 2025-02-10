using System.Threading.Tasks;

using Microsoft.Playwright;
namespace Alstom.Spectrail.Framework.Actions
{
    public class WaitForVisibilityHandler : BaseActionHandler
    {
        private readonly IPage _page;
        private readonly string _selector;
        public WaitForVisibilityHandler(IPage page, string selector)
        {
            _page = page;
            _selector = selector;
        }
        protected override async Task ExecuteAsync()
        {
            await _page.WaitForSelectorAsync(_selector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
        }
    }
}