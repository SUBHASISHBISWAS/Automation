using System.Threading.Tasks;
using Microsoft.Playwright;
using Alstom.Spectrail.Framework.PageObjects;
using Alstom.Spectrail.Framework.Decorators;
using Alstom.Spectrail.Framework.Utilities;

namespace Alstom.Spectrail.Framework.Actions
{
    [ApplyLogging]
    [ApplyScreenshot]
    [ApplyVideo] // ✅ Automatically enables video recording for failures
    public class LoginHandler : BaseActionHandler
    {
        private readonly LoginPage _loginPage;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private bool _verifyLink;

        /// <summary>
        /// Constructor for LoginHandler.
        /// </summary>
        public LoginHandler(ActionFactory actionFactory) : base(actionFactory)
        {
            _loginPage = _actionFactory.CreatePage<LoginPage>(); // ✅ Automatically retrieves LoginPage
        }
        public override IPage? Page => _loginPage.Page; // ✅ Now properly exposes Playwright Page

        public LoginHandler WithUsername(string username)
        {
            _username = username;
            return this;
        }

        public LoginHandler WithPassword(string password)
        {
            _password = password;
            return this;
        }

        public LoginHandler VerifyNavigation(bool verify = true)
        {
            _verifyLink = verify;
            return this;
        }

        protected override async Task ExecuteAsync()
        {
            await WaitForSubmitButton();
            await ThenLogin();
        }

        /// <summary>
        /// Waits for the submit button to become visible using Playwright’s built-in waiting.
        /// </summary>
        private async Task<LoginHandler> WaitForSubmitButton()
        {
            await _loginPage.WaitForSubmitButton(); // ✅ Uses Playwright's built-in wait instead of `WaitForConditionAsync()`
            return this;
        }

        private async Task<LoginHandler> ThenLogin()
        {
            await _loginPage.Login(_username, _password);
            return this;
        }

        private async Task<LoginHandler> ThenVerifyIfRequired()
        {
            if (_verifyLink)
            {
                await _loginPage.LoginAndVerifyLink(_username, _password);
            }
            return this;
        }
    }
}