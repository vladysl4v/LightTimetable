using System.Windows;
using System.Threading.Tasks;

using LightTimetable.Tools;
using LightTimetable.Tools.Data;
using LightTimetable.Tools.Electricity;


namespace LightTimetable.ViewModels
{ 
    /// <summary>
    /// Provides bindable properties and commands for the NotifyIcon.
    /// </summary>
    public class NotifyIconViewModel
    {
        private SettingsView? _settingsWindow;

        private bool _isWindowInitialized;

        public NotifyIconViewModel()
        {
            InitializeNotifyIcon();
        }

        private async Task InitializeNotifyIcon()
        {
            await DataProvider.InitializeDataAsync();
            await ElectricityProvider.InitializeBlackoutsAsync();

            _isWindowInitialized = true;
            Application.Current.MainWindow = new TimetableView();
        }

        #region Commands

        private RelayCommand _closeApplicationCommand = null!;
        private RelayCommand _showTimetableCommand = null!;
        private RelayCommand _openSettingsCommand = null!;
        private RelayCommand _refreshDataCommand = null!;


        public RelayCommand RefreshDataCommand
        {
            get
            {
                return _refreshDataCommand ??= new RelayCommand(async obj =>
                {
                    await DataProvider.InitializeDataAsync();
                    await ElectricityProvider.InitializeBlackoutsAsync();
                    ((TimetableView)Application.Current.MainWindow).ReloadData();
                    ((TimetableView)Application.Current.MainWindow).RefreshTimetable();
                });
            }
        }

        public RelayCommand OpenSettingsCommand
        {
            get
            {
                return _openSettingsCommand ??= new RelayCommand(obj =>
                {
                    if (_settingsWindow == null)
                    {
                        _settingsWindow = new SettingsView();
                        _settingsWindow.Closed += (s, e) => _settingsWindow = null;
                        _settingsWindow.Show();
                    }
                });
            }
        }

        public RelayCommand ShowTimetableCommand
        {
            get
            {
                return _showTimetableCommand ??= new RelayCommand(obj =>
                {
                    if (_isWindowInitialized)
                        Application.Current.MainWindow.Show();
                });
            }
        }

        public RelayCommand CloseApplicationCommand
        {
            get
            {
                return _closeApplicationCommand ??= new RelayCommand(obj =>
                {
                    Application.Current.Shutdown();
                });
            }
        }
        #endregion
    }
}
