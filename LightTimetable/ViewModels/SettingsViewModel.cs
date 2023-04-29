using LightTimetable.Tools;
using LightTimetable.SettingsPages.ViewModels;


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
            IntegrationsCategoryCommand = new RelayCommand(_ => SetIntegrationsCategory());

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
        public RelayCommand IntegrationsCategoryCommand { get; }

        private void SetApplicationCategory() => CurrentView = new ApplicationPageViewModel();
        private void SetScheduleCategory() => CurrentView = new SchedulePageViewModel(true);
        private void SetRenamingCategory() => CurrentView = new RenamingPageViewModel();
        private void SetIntegrationsCategory() => CurrentView = new IntegrationsPageViewModel();
        #endregion
    }
}
