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
            CheckIsOutagesSetUp();
            IsAnythingChanged = false;
        }

        #region Properties

        public string AuthText { get; set; }
        
        public string AuthLogin { get; set; }
        
        public bool IsAuthenticated { get; set; }

        public string AuthButtonTitle { get; set; }
        
        [ObservableProperty]
        public bool _isOutagesSetUp;

        [ObservableProperty]
        private bool _showTeamsEvents = Settings.Default.ShowTeamsEvents;

        [ObservableProperty]
        private bool _showOutages = Settings.Default.ShowOutages;
        
        [ObservableProperty]
        private bool _showPossibleOutages = Settings.Default.ShowPossibleOutages;
        
        [ObservableProperty]
        private bool _showOldEvents = Settings.Default.ShowOldEvents;

        private int _outagesGroup = Settings.Default.OutagesGroup;

        private string _outagesCity = Settings.Default.OutagesCity;

        public string OutagesCity
        {
            get => _outagesCity;
            set
            {
                SetProperty(ref _outagesCity, value);
                CheckIsOutagesSetUp();
            }
        }

        public string OutagesGroup
        {
            get => _outagesGroup.ToString();
            set
            {
                SetProperty(ref _outagesGroup, int.Parse(value));
                CheckIsOutagesSetUp();
            }
        }
        
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
            var oldOutagesValue = Settings.Default.ShowOutages;

            Settings.Default.ShowOutages = ShowOutages;
            Settings.Default.ShowPossibleOutages = ShowPossibleOutages;
            Settings.Default.OutagesGroup = _outagesGroup;
            Settings.Default.OutagesCity = _outagesCity;
            Settings.Default.ShowTeamsEvents = ShowTeamsEvents;
            Settings.Default.ShowOldEvents = ShowOldEvents;
            
            Settings.Default.Save();

            if (oldOutagesValue != ShowOutages)
            {
                SettingsView.IsRequiredResize = true;
            }

            IsAnythingChanged = false;
        }

        private void CheckIsOutagesSetUp()
        {
            IsOutagesSetUp = _outagesGroup != 0 && _outagesCity != string.Empty;
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
