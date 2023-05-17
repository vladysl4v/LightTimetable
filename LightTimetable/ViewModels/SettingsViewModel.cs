using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

using LightTimetable.SettingsPages.ViewModels;


namespace LightTimetable.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        public SettingsViewModel()
        {
            // Startup page
            CurrentView = new ApplicationPageViewModel();
        }

        #region Properties

        [ObservableProperty]
        private PageViewModelBase _currentView;

        #endregion

        #region Commands

        [RelayCommand]
        private void ApplicationCategory() => CurrentView = new ApplicationPageViewModel();
        
        [RelayCommand]
        private void ScheduleCategory() => CurrentView = new SchedulePageViewModel(true);

        [RelayCommand]
        private void RenamingCategory() => CurrentView = new RenamingPageViewModel();
        
        [RelayCommand]
        private void IntegrationsCategory() => CurrentView = new IntegrationsPageViewModel();
 
        #endregion
    }
}
