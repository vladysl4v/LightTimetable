using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

using System.Threading.Tasks;

using LightTimetable.Views;
using LightTimetable.Properties;
using LightTimetable.Models.Services;


namespace LightTimetable.SettingsPages.ViewModels
{
    public partial class IntegrationsPageViewModel : PageViewModelBase
    {
        public IntegrationsPageViewModel()
        {
            CheckForAuthStatus();
            IsAnythingChanged = false;
        }

        #region Properties

        public string AuthText { get; set; }
        
        public string AuthLogin { get; set; }
        
        private bool IsAuthenticated { get; set; }
        public string AuthButtonTitle { get; set; }

        [ObservableProperty]
        private bool _showTeamsEvents = Settings.Default.ShowTeamsEvents;
        
        [ObservableProperty]
        private string _blackoutsGroup = Settings.Default.DTEKGroup;
        
        [ObservableProperty]
        private bool _showBlackouts = Settings.Default.ShowBlackouts;
        
        [ObservableProperty]
        private bool _showPossibleBlackouts = Settings.Default.ShowPossibleBlackouts;
        
        [ObservableProperty]
        private bool _showOldEvents = Settings.Default.ShowOldEvents;

        #endregion

        #region Commands

        [RelayCommand]
        public async Task Authorize()
        {
            if (IsAuthenticated)
            {
                var isSuccessful = await TeamsEventsPlugin.SignOutAsync();
                if (isSuccessful)
                {
                    ChangeAuthStatus(false);
                }
                return;
            }

            var auth = await TeamsEventsPlugin.AuthorizeInteractiveAsync();
            if (auth == null)
            {
                ChangeAuthStatus(false);
            }
            else
            {
                ChangeAuthStatus(true, auth.Account.Username);
            }
        }

        #endregion

        #region Methods

        public override void Save()
        {
            if (IsAnythingChanged)
            {
                SettingsView.IsRequiredReload = true;
            }
            var oldBlackoutsValue = Settings.Default.ShowBlackouts;

            Settings.Default.ShowBlackouts = ShowBlackouts;
            Settings.Default.ShowPossibleBlackouts = ShowPossibleBlackouts;
            Settings.Default.DTEKGroup = BlackoutsGroup;
            Settings.Default.ShowTeamsEvents = ShowTeamsEvents;
            Settings.Default.ShowOldEvents = ShowOldEvents;
            
            Settings.Default.Save();

            if (oldBlackoutsValue != ShowBlackouts)
            {
                SettingsView.IsRequiredResize = true;
            }

            IsAnythingChanged = false;
        }

        private async void CheckForAuthStatus()
        {
            var tryAuthorize = await TeamsEventsPlugin.AuthorizeSilentAsync();

            if (tryAuthorize != null)
            {
                ChangeAuthStatus(true, tryAuthorize.Account.Username);
            }
            else
            {
                ChangeAuthStatus(false);
            }
        }

        private void ChangeAuthStatus(bool isAuth, string email = "")
        {
            AuthText = isAuth
                ? $"Ви увійшли до облікового запису"
                : "Ви не увійшли до облікового запису";
            AuthLogin = email;
            AuthButtonTitle = isAuth
                ? "Вийти"
                : "Авторизуватись";

            IsAuthenticated = isAuth;

            OnPropertyChanged(nameof(AuthText));
            OnPropertyChanged(nameof(AuthLogin));
            OnPropertyChanged(nameof(AuthButtonTitle));
            OnPropertyChanged(nameof(IsAuthenticated));
        }

        #endregion 

    }

}
