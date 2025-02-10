using System.Threading.Tasks;

using Alstom.Spectrail.Framework.PageObjects;
using Alstom.Spectrail.Framework.Utilities;

using Microsoft.Playwright;

namespace Alstom.Spectrail.Framework.Actions
{
    public class OpenPageHandler : BaseActionHandler
    {
        private readonly IPage _page;
        private readonly string _url;

        public OpenPageHandler(ActionFactory actionFactory, string url)
            : base(actionFactory)
        {
            _page = _actionFactory.CreatePage<BasePage>().Page; // ✅ Gets the Playwright page dynamically
            _url = url;
        }

        protected override async Task ExecuteAsync()
        {
            if (_page != null)
            {
                await _page.GotoAsync(_url);
            }
            else
            {
                throw new System.InvalidOperationException("Page instance is null. Cannot navigate.");
            }
        }

        public override IPage? Page => _page; // ✅ Exposes Playwright Page for decorators
    }
}