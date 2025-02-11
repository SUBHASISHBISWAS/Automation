using System.Threading.Tasks;

using Microsoft.Playwright;

namespace Alstom.Spectrail.Framework.PageObjects
{
    public class LoginPage(IPage page) : BasePage(page)
    {
        private ILocator _userName => Page.GetByLabel("Username");
        private ILocator _password => Page.GetByLabel("Password");
        private ILocator _submitButton => Page.GetByRole(AriaRole.Button, new() { Name = "Submit" });
        private ILocator _loggedInSuccessfully => Page.GetByRole(AriaRole.Heading, new() { Name = "Logged In Successfully" });

        /// <summary>
        /// Waits for the submit button to be visible.
        /// </summary>
        public async Task WaitForSubmitButton()
        {
            await _submitButton.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
        }

        /// <summary>
        /// Performs a login action.
        /// </summary>
        public async Task Login(string userName, string password)
        {
            await _userName.FillAsync(userName);
            await _password.FillAsync(password);
            await _submitButton.ClickAsync();
        }

        /// <summary>
        /// Performs login and verifies successful navigation.
        /// </summary>
        public async Task LoginAndVerifyLink(string userName, string password)
        {
            await _userName.FillAsync(userName);
            await _password.FillAsync(password);
            await Page.RunAndWaitForNavigationAsync(async () => { await _submitButton.ClickAsync(); },
                new PageRunAndWaitForNavigationOptions()
                {
                    UrlString = "https://practicetestautomation.com/logged-in-successfully/"
                });
        }

        /// <summary>
        /// Checks if the submit button is visible.
        /// </summary>
        public async Task<bool> IsSubmitButtonVisible() => await _submitButton.IsVisibleAsync();

        /// <summary>
        /// Checks if login was successful.
        /// </summary>
        public async Task<bool> IsLoggedIn() => await _loggedInSuccessfully.IsVisibleAsync();
    }
}