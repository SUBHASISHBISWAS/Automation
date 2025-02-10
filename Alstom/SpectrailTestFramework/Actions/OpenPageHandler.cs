using System.Threading.Tasks;
using Microsoft.Playwright;
using Alstom.Spectrail.Framework.Actions;
using Alstom.Spectrail.Framework.Utilities;

namespace Alstom.Spectrail.Framework.Actions
{
    public class OpenPageHandler : BaseActionHandler
    {
        private readonly IPage _page;
        private string _url;

        public OpenPageHandler(ActionFactory actionFactory, IPage page) : base(actionFactory)
        {
            _page = page;
        }

        public OpenPageHandler WithUrl(string url)
        {
            _url = url;
            return this; // ✅ Enables fluent chaining
        }

        protected override async Task ExecuteAsync()
        {
            if (string.IsNullOrEmpty(_url))
                throw new InvalidOperationException("URL must be set before executing OpenPageHandler.");

            await _page.GotoAsync(_url);
        }
    }
}