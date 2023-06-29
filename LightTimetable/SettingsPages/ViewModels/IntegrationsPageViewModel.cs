using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

using System;
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

        // System.Tuple is used due to the presence of properties that the ValueTuple doesnt have
        public static Tuple<string?, string, string> TeamsCredentials { get; set; }

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
            if (TeamsCredentials.Item1 == null)
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
            TeamsCredentials = new Tuple<string?, string, string>
            (
                email,
                isAuth ? "Ви увійшли до облікового запису" : "Ви не увійшли до облікового запису",
                isAuth ? "Вийти" : "Авторизуватись"
            );
            OnPropertyChanged(nameof(TeamsCredentials));
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
