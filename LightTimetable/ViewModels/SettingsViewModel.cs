using System.Windows;

using LightTimetable.SettingsPages.ViewModels;
using LightTimetable.Tools;


namespace LightTimetable.ViewModels
{
    class SettingsViewModel : ViewModelBase
    {
        private PageViewModelBase _currentView;

        public PageViewModelBase CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public SettingsViewModel()
        {
            // Commands
            SaveAndCloseCommand = new RelayCommand(SaveAndCloseSettings);
            CloseSettingsCommand = new RelayCommand(CloseSettings);
            SaveSettingsCommand = new RelayCommand(SaveSettings);

            ApplicationCategoryCommand = new RelayCommand(_ => SetApplicationCategory());
            ScheduleCategoryCommand = new RelayCommand(_ => SetScheduleCategory());
            RenamingCategoryCommand = new RelayCommand(_ => SetRenamingCategory());

            // Startup page
            CurrentView = new ApplicationPageViewModel();
        }

        // Button commands
        public RelayCommand SaveSettingsCommand { get; }
        public RelayCommand SaveAndCloseCommand { get; }
        public RelayCommand CloseSettingsCommand { get; }

        private void SaveSettings(object win)
        {
            if (win is not Window thisWindow) return;
            CurrentView.Save();
        }

        private void SaveAndCloseSettings(object win)
        {
            if (win is not Window thisWindow) return;
            CurrentView.Save();
            thisWindow.Close();
        }

        private void CloseSettings(object win)
        {
            if (win is not Window thisWindow) return;

            if (!CurrentView.IsAnythingChanged)
            {
                thisWindow.Close();
            }
            var msgResult = MessageBox.Show("Ви внесли не збережені зміни. Все одно закрити налаштування?", "Налаштування",
                    MessageBoxButton.YesNo);
            if (msgResult == MessageBoxResult.Yes)
            {
                thisWindow.Close();
            }
        }

        // Categories commands
        public RelayCommand ApplicationCategoryCommand { get; }
        public RelayCommand ScheduleCategoryCommand { get; }
        public RelayCommand RenamingCategoryCommand { get; }

        private void SetApplicationCategory() => CurrentView = new ApplicationPageViewModel();
        private void SetScheduleCategory() => CurrentView = new SchedulePageViewModel();
        private void SetRenamingCategory() => CurrentView = new RenamingPageViewModel();
    }
}
