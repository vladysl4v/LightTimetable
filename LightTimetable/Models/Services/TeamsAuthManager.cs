using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Authentication;

using System;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Tools;


namespace LightTimetable.Models.Services
{
    public static class TeamsAuthManager
    {
        private static readonly IPublicClientApplication? ClientApp;
        private const string AppId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";

        static TeamsAuthManager()
        {
            try
            {
                ClientApp = PublicClientApplicationBuilder.Create(AppId).Build();
                TeamsCacheManager.EnableSerialization(ClientApp.UserTokenCache);
            }
            catch
            {
                ClientApp = null;
            }          
        }

        public static BaseBearerTokenAuthenticationProvider GetAuthenticationProvider()
        {
            return new BaseBearerTokenAuthenticationProvider(new TeamsTokenProvider());
        }

        public static async Task<AuthenticationResult?> RequestAuthenticationToken()
        {
            if (ClientApp == null)
                return null;

            return await AuthorizeSilentAsync() ?? await AuthorizeInteractiveAsync();
        }

        public static async Task<AuthenticationResult?> AuthorizeInteractiveAsync()
        {
            if (ClientApp == null)
                return null;

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
            if (ClientApp == null)
                return null;

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
            if (ClientApp == null)
                return false;

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

        private class TeamsTokenProvider : IAccessTokenProvider
        {
            public async Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object>? authContext, CancellationToken cancellationToken)
            {
                var authResult = await RequestAuthenticationToken();

                return authResult?.AccessToken;
            }

            public AllowedHostsValidator AllowedHostsValidator => null!;
        }
    }
}