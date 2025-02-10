using System.Threading.Tasks;

using Alstom.Spectrail.Framework.PageObjects;
using Alstom.Spectrail.Framework.Utilities;

using Microsoft.Playwright;

namespace Alstom.Spectrail.Framework.Actions
{
    public class WaitForVisibilityHandler : BaseActionHandler
    {
        private readonly IPage _page;
        private readonly string _selector;

        public WaitForVisibilityHandler(ActionFactory actionFactory, string selector)
            : base(actionFactory)
        {
            _page = _actionFactory.CreatePage<BasePage>().Page; // ✅ Dynamically retrieves the Playwright page
            _selector = selector;
        }

        protected override async Task ExecuteAsync()
        {
            if (_page != null)
            {
                await _page.WaitForSelectorAsync(_selector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            }
            else
            {
                throw new System.InvalidOperationException("Page instance is null. Cannot wait for selector.");
            }
        }

        public override IPage? Page => _page; // ✅ Exposes Playwright Page for decorators
    }
}