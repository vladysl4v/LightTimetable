using Microsoft.Identity.Client;

using System;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;

using LightTimetable.Tools;


namespace LightTimetable.Models.Services
{
    public static class TeamsAuthManager
    {
        private static readonly IPublicClientApplication ClientApp;
        private const string AppId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";

        static TeamsAuthManager()
        {
            ClientApp = PublicClientApplicationBuilder.Create(AppId).Build();
            TeamsCacheManager.EnableSerialization(ClientApp.UserTokenCache);
        }

        public static async Task<AuthenticationResult?> RequestAuthenticationToken()
        {
            return await AuthorizeSilentAsync() ?? await AuthorizeInteractiveAsync();
        }

        public static async Task<AuthenticationResult?> AuthorizeInteractiveAsync()
        {
            string[] scopes = { "Calendars.Read.Shared" };
            try
            {
                var result = await ClientApp.AcquireTokenInteractive(scopes).ExecuteAsync();
                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show("Виникла помилка під час авторизації:\n" + e.Message, "LightTimetable", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

        }

        public static async Task<AuthenticationResult?> AuthorizeSilentAsync()
        {
            string[] scopes = { "Calendars.Read.Shared" };

            var accounts = await ClientApp.GetAccountsAsync();

            try
            {
                return await ClientApp.AcquireTokenSilent(scopes, accounts.FirstOrDefault()).ExecuteAsync();
            }
            catch (MsalUiRequiredException)
            {
                return null;
            }
        }

        public static async Task<bool> SignOutAsync()
        {
            var accounts = await ClientApp.GetAccountsAsync();
            try
            {
                await ClientApp.RemoveAsync(accounts.FirstOrDefault());
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Виникла помилка під час виходу:\n" + e.Message, "LightTimetable", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}