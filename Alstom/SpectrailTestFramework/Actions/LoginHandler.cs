using System.Threading.Tasks;
using Microsoft.Playwright;
using Alstom.Spectrail.Framework.PageObjects;
namespace Alstom.Spectrail.Framework.Actions
{
    public class LoginHandler : BaseActionHandler
    {
        private readonly LoginPage _loginPage;
        private readonly string _username;
        private readonly string _password;
        public LoginHandler(LoginPage loginPage, string username, string password)
        {
            _loginPage = loginPage;
            _username = username;
            _password = password;
        }
        protected override async Task ExecuteAsync()
        {
            await _loginPage.LoginAsync(_username, _password);
        }
    }
}