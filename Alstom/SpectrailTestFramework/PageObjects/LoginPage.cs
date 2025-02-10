using Microsoft.Playwright;

using System.Threading.Tasks;
namespace Alstom.Spectrail.Framework.PageObjects
{
    public class LoginPage : BasePage
    {
        public LoginPage(IPage page) : base(page) { }
        public async Task LoginAsync(string username, string password)
        {
            await Page.FillAsync("#username", username);
            await Page.FillAsync("#password", password);
            await Page.ClickAsync("#loginButton");
        }
    }
}