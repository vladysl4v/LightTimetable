using System.Threading.Tasks;

using LightTimetable.Tools;
using LightTimetable.Views;
using LightTimetable.Properties;
using LightTimetable.Models.Services;


namespace LightTimetable.SettingsPages.ViewModels
{
    public class IntegrationsPageViewModel : PageViewModelBase
    {
        public IntegrationsPageViewModel()
        {
            CheckForAuthStatus();

            AuthorizeCommand = new RelayCommand(async _ => await AuthorizeAccount());
        }

        #region Properties

        private bool _showTeamsEvents = Settings.Default.ShowTeamsEvents;
        private string _blackoutsGroup = Settings.Default.DTEKGroup;
        private bool _showBlackouts = Settings.Default.ShowBlackouts;
        private bool _showPossibleBlackouts = Settings.Default.ShowPossibleBlackouts;

        private string _authText;
        private string _authLogin;
        private string _authButtonTitle;
        private bool _isAuthenticated;

        public string AuthText => _authText;
        public string AuthButtonTitle => _authButtonTitle;
        public string AuthLogin => _authLogin;

        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            set => SetProperty(ref _isAuthenticated, value);
        }

        public bool ShowTeamsEvents
        {
            get => _showTeamsEvents;
            set => SetProperty(ref _showTeamsEvents, value);
        }

        public bool ShowBlackouts
        {
            get => _showBlackouts;
            set => SetProperty(ref _showBlackouts, value);
        }

        public bool ShowPossibleBlackouts
        {
            get => _showPossibleBlackouts;
            set => SetProperty(ref _showPossibleBlackouts, value);
        }

        public string BlackoutsGroup
        {
            get => _blackoutsGroup;
            set => SetProperty(ref _blackoutsGroup, value);
        }

        #endregion

        #region Commands

        public RelayCommand AuthorizeCommand { get; }

        public async Task AuthorizeAccount()
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
            Settings.Default.ShowBlackouts = ShowBlackouts;
            Settings.Default.ShowPossibleBlackouts = ShowPossibleBlackouts;
            Settings.Default.DTEKGroup = BlackoutsGroup;
            Settings.Default.ShowTeamsEvents = ShowTeamsEvents;
            Settings.Default.Save();
            IsAnythingChanged = false;
        }

        private async void CheckForAuthStatus()
        {
            var abc = await TeamsEventsPlugin.AuthorizeSilentAsync();

            if (abc != null)
            {
                ChangeAuthStatus(true, abc.Account.Username);
            }
            else
            {
                ChangeAuthStatus(false);
            }
        }

        private void ChangeAuthStatus(bool isAuth, string email = "")
        {
            _authText = isAuth
                ? $"Ви увійшли до облікового запису"
                : "Ви не увійшли до облікового запису";
            _authLogin = email;
            _authButtonTitle = isAuth
                ? "Вийти"
                : "Авторизуватись";

            IsAuthenticated = isAuth;

            OnPropertyChanged(nameof(AuthText));
            OnPropertyChanged(nameof(AuthLogin));
            OnPropertyChanged(nameof(AuthButtonTitle));
        }

        #endregion 

    }

}
