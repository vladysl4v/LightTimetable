using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

using System.ComponentModel;
using System.Threading.Tasks;

using LightTimetable.Models;
using LightTimetable.Services;
using LightTimetable.Properties;


namespace LightTimetable.ViewModels.Pages
{
    public partial class IntegrationsPageViewModel : PageViewModelBase
    {
        private readonly UpdatesMediator _mediator;
        private readonly IUserSettings _settings;
        public IntegrationsPageViewModel(IUserSettings settings, UpdatesMediator mediator)
        {
            _settings = settings;
            _mediator = mediator;

            PropertyChanged += SomethingChanged;
            
            Task.Run(GetAuthorizationStateAsync).ConfigureAwait(false);

            _showTeamsEvents = _settings.ShowTeamsEvents;
            _showOutages = _settings.ShowOutages;
            _showPossibleOutages = settings.ShowPossibleOutages;
            _showOldEvents = _settings.ShowOldEvents;
            //_outagesCity = _settings.OutagesCity;
            _outagesGroup = _settings.OutagesGroup.ToString();

            IsOutagesSetUp = OutagesGroup != "0" && OutagesGroup != string.Empty; // && OutagesCity != string.Empty;
        }

        #region Properties

        [ObservableProperty]
        private string? _teamsLogin;

        [ObservableProperty]
        private string _teamsText;

        [ObservableProperty]
        private string _teamsTitle = "Завантаження...";

        [ObservableProperty]
        private bool _showTeamsEvents;

        [ObservableProperty]
        private bool _showOutages;
        
        [ObservableProperty]
        private bool _showPossibleOutages;
        
        [ObservableProperty]
        private bool _showOldEvents;

        //[ObservableProperty]
        //private string _outagesCity;

        [ObservableProperty]
        private string _outagesGroup;

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

            _settings.ShowOutages = ShowOutages;
            _settings.ShowPossibleOutages = ShowPossibleOutages;
            _settings.OutagesGroup = int.Parse(OutagesGroup);
            //_settings.OutagesCity = OutagesCity;
            _settings.ShowTeamsEvents = ShowTeamsEvents;
            _settings.ShowOldEvents = ShowOldEvents;
            _settings.Save();
            
            if (isSettingsChanged)
            {
                _mediator.ReloadRequired();
                _mediator.RepositionRequired();
            }
        }

        private bool IsSettingsChanged()
        {
            return _settings.ShowPossibleOutages != ShowPossibleOutages ||
                   _settings.OutagesGroup != int.Parse(OutagesGroup) ||
                   _settings.ShowTeamsEvents != ShowTeamsEvents ||
                   //_settings.OutagesCity != OutagesCity ||
                   _settings.ShowOutages != ShowOutages ||
                   _settings.ShowOldEvents != ShowOldEvents;
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
            if (args.PropertyName is nameof(OutagesGroup)) //or nameof(OutagesCity))
            {
                IsOutagesSetUp = OutagesGroup != "0" && OutagesGroup != string.Empty; // && OutagesCity != string.Empty;
            }
        }
        #endregion 
    }
}
