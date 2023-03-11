using System.Windows;
using System.Threading.Tasks;

using LightTimetable.Model;
using LightTimetable.Tools;
using LightTimetable.Model.Electricity;

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

            ShowTimetableCommand = new RelayCommand(_ => ShowTimetable());
            RefreshDataCommand = new RelayCommand(_ => RefreshData());
            OpenSettingsCommand = new RelayCommand(_ => OpenSettings());
            CloseApplicationCommand = new RelayCommand(_ => CloseApplication());
        }

        private async Task InitializeNotifyIcon()
        {
            await DataProvider.InitializeDataAsync();
            await ElectricityProvider.InitializeBlackoutsAsync();

            _isWindowInitialized = true;
            Application.Current.MainWindow = new TimetableView();
        }

        #region Commands
        public RelayCommand ShowTimetableCommand { get; }
        public RelayCommand RefreshDataCommand { get; }
        public RelayCommand OpenSettingsCommand { get; }
        public RelayCommand CloseApplicationCommand { get; }

        private void ShowTimetable()
        {
            if (_isWindowInitialized)
                Application.Current.MainWindow.Show();
        }

        private async void RefreshData()
        {
            await DataProvider.InitializeDataAsync();
            ((TimetableView)Application.Current.MainWindow).ReloadData();
            ((TimetableView)Application.Current.MainWindow).RefreshTimetable();
        }

        private void OpenSettings()
        {
            if (_settingsWindow == null)
            {
                _settingsWindow = new SettingsView();
                _settingsWindow.Closed += (s, e) => _settingsWindow = null;
                _settingsWindow.Show();
            }
        }

        private void CloseApplication()
        {
            Application.Current.Shutdown();
        }

        #endregion
    }
}
