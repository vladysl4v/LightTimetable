using System.Windows;

using LightTimetable.SettingsPages.ViewModels;
using LightTimetable.Tools;


namespace LightTimetable.ViewModels
{
    class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel()
        {
            // Commands
            ApplicationCategoryCommand = new RelayCommand(_ => SetApplicationCategory());
            ScheduleCategoryCommand = new RelayCommand(_ => SetScheduleCategory());
            RenamingCategoryCommand = new RelayCommand(_ => SetRenamingCategory());

            // Startup page
            CurrentView = new ApplicationPageViewModel();
        }

        #region Properties

        private PageViewModelBase _currentView;
        public PageViewModelBase CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        #endregion

        #region Commands

        public RelayCommand ApplicationCategoryCommand { get; }
        public RelayCommand ScheduleCategoryCommand { get; }
        public RelayCommand RenamingCategoryCommand { get; }

        private void SetApplicationCategory() => CurrentView = new ApplicationPageViewModel();
        private void SetScheduleCategory() => CurrentView = new SchedulePageViewModel();
        private void SetRenamingCategory() => CurrentView = new RenamingPageViewModel();

        #endregion
    }
}
