using Microsoft.Playwright;

using System.Threading.Tasks;
namespace Alstom.Spectrail.Framework.PageObjects
{
    public abstract class BasePage
    {
        protected readonly IPage Page;
        protected BasePage(IPage page)
        {
            Page = page;
        }
        public async Task GoToUrl(string url)
        {
            await Page.GotoAsync(url);
        }
    }
}