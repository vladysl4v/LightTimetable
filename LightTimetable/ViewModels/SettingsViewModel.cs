using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

using LightTimetable.Models;
using LightTimetable.ViewModels.Pages;


namespace LightTimetable.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly CreateViewModel<ApplicationPageViewModel> _createApplicationPageViewModel;
        private readonly CreateViewModel<IntegrationsPageViewModel> _createIntegrationsPageViewModel;
        private readonly CreateViewModel<RenamingPageViewModel> _createRenamingPageViewModel;
        private readonly CreateViewModel<SchedulePageViewModel> _createSchedulePageViewModel;

        public SettingsViewModel(CreateViewModel<ApplicationPageViewModel> createApplicationPageViewModel,
                                 CreateViewModel<IntegrationsPageViewModel> createIntegrationsPageViewModel,
                                 CreateViewModel<RenamingPageViewModel> createRenamingPageViewModel,
                                 CreateViewModel<SchedulePageViewModel> createSchedulePageViewModel)
        {
            _createApplicationPageViewModel = createApplicationPageViewModel;
            _createIntegrationsPageViewModel = createIntegrationsPageViewModel;
            _createRenamingPageViewModel = createRenamingPageViewModel;
            _createSchedulePageViewModel = createSchedulePageViewModel;

            // Startup page
            CurrentView = _createApplicationPageViewModel.Invoke();
        }

        [ObservableProperty]
        private PageViewModelBase _currentView;

        [RelayCommand]
        private void ApplicationCategory() => CurrentView = _createApplicationPageViewModel.Invoke();
        
        [RelayCommand]
        private void ScheduleCategory() => CurrentView = _createSchedulePageViewModel.Invoke();

        [RelayCommand]
        private void RenamingCategory() => CurrentView = _createRenamingPageViewModel.Invoke();
        
        [RelayCommand]
        private void IntegrationsCategory() => CurrentView = _createIntegrationsPageViewModel.Invoke();
    }
}
