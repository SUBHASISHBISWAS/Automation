using System.Threading.Tasks;

using Microsoft.Playwright;
namespace Alstom.Spectrail.Framework.Actions
{
    public class OpenPageHandler : BaseActionHandler
    {
        private readonly IPage _page;
        private readonly string _url;
        public OpenPageHandler(IPage page, string url)
        {
            _page = page;
            _url = url;
        }
        protected override async Task ExecuteAsync()
        {
            await _page.GotoAsync(_url);
        }
    }
}