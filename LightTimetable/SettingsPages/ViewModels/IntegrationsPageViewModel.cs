using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

using System.ComponentModel;
using System.Threading.Tasks;

using LightTimetable.Tools;
using LightTimetable.Properties;
using LightTimetable.Models.Services;


namespace LightTimetable.SettingsPages.ViewModels
{
    public partial class IntegrationsPageViewModel : PageViewModelBase
    {
        public IntegrationsPageViewModel()
        {
            PropertyChanged += SomethingChanged;
            
            Task.Run(GetAuthorizationStateAsync).ConfigureAwait(false);

            IsOutagesSetUp = OutagesGroup != "0" && OutagesGroup != string.Empty 
                            && OutagesCity != string.Empty;
        }

        #region Properties

        [ObservableProperty]
        public string? _teamsLogin;

        [ObservableProperty]
        public string _teamsText;

        [ObservableProperty]
        public string _teamsTitle = "Завантаження...";

        [ObservableProperty]
        private bool _showTeamsEvents = Settings.Default.ShowTeamsEvents;

        [ObservableProperty]
        private bool _showOutages = Settings.Default.ShowOutages;
        
        [ObservableProperty]
        private bool _showPossibleOutages = Settings.Default.ShowPossibleOutages;
        
        [ObservableProperty]
        private bool _showOldEvents = Settings.Default.ShowOldEvents;

        [ObservableProperty]
        private string _outagesCity = Settings.Default.OutagesCity;

        [ObservableProperty]
        private string _outagesGroup = Settings.Default.OutagesGroup.ToString();

        [ObservableProperty]
        private bool _isTeamsSetUp;
        
        [ObservableProperty]
        private bool _isOutagesSetUp;

        #endregion

        #region Commands

        [RelayCommand]
        public async Task Authorize()
        {
            if (TeamsLogin == null)
            {       
                var authorizeResult = await TeamsAuthManager.AuthorizeInteractiveAsync();
                
                if (authorizeResult != null)
                {
                    ChangeAuthState(true, authorizeResult.Account.Username);
                }
            }
            else
            {
                var isSuccessful = await TeamsAuthManager.SignOutAsync();
                if (isSuccessful)
                {
                    ChangeAuthState(false);
                }
            }
        }

        #endregion

        #region Methods

        public override void Save()
        {
            var isSettingsChanged = IsSettingsChanged();

            Settings.Default.ShowOutages = ShowOutages;
            Settings.Default.ShowPossibleOutages = ShowPossibleOutages;
            Settings.Default.OutagesGroup = int.Parse(_outagesGroup);
            Settings.Default.OutagesCity = _outagesCity;
            Settings.Default.ShowTeamsEvents = ShowTeamsEvents;
            Settings.Default.ShowOldEvents = ShowOldEvents;
            Settings.Default.Save();
            
            if (isSettingsChanged)
            {
                WindowMediator.ReloadRequired();
                WindowMediator.RepositionRequired();
            }
        }

        private bool IsSettingsChanged()
        {
            return Settings.Default.ShowPossibleOutages != ShowPossibleOutages ||
                   Settings.Default.OutagesGroup != int.Parse(_outagesGroup) ||
                   Settings.Default.ShowTeamsEvents != ShowTeamsEvents ||
                   Settings.Default.OutagesCity != _outagesCity ||
                   Settings.Default.ShowOutages != ShowOutages ||
                   Settings.Default.ShowOldEvents != ShowOldEvents;
        }

        private async Task GetAuthorizationStateAsync()
        {
            var authorizeResult = await TeamsAuthManager.AuthorizeSilentAsync();

            if (authorizeResult != null)
            {
                ChangeAuthState(true, authorizeResult.Account.Username);
            }
            else
            {
                ChangeAuthState(false);
            }
        }

        private void ChangeAuthState(bool isAuth, string? email = null)
        {
            TeamsLogin = email;
            TeamsText = isAuth ? "Ви увійшли до облікового запису" : "Ви не увійшли до облікового запису";
            TeamsTitle = isAuth ? "Вийти" : "Авторизуватись";
        
            IsTeamsSetUp = email != null;
        }

        private void SomethingChanged(object? sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName is nameof(OutagesGroup) or nameof(OutagesCity))
            {
                IsOutagesSetUp = OutagesGroup != "0" && OutagesGroup != string.Empty 
                                 && OutagesCity != string.Empty;
            }
        }
        #endregion 
    }
}
