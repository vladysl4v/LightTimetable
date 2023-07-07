using CommunityToolkit.Mvvm.ComponentModel;

using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Tools;
using LightTimetable.Models;
using LightTimetable.Properties;
using LightTimetable.DataTypes.Interfaces;

namespace LightTimetable.SettingsPages.ViewModels
{
    public partial class SchedulePageViewModel : PageViewModelBase
    {
        private IScheduleSettings _settingsSource;
        public SchedulePageViewModel(bool disableInitialization) { }
        public SchedulePageViewModel()
        {
            PropertyChanged += SomethingChanged;

            ScheduleSourceSource = ScheduleReflector.GetScheduleNames();
            
            Task.Run(InitializeSettings).ConfigureAwait(false);
        }

        #region Properties

        
        [ObservableProperty]
        private static bool _facultiesVisibility;
        
        [ObservableProperty]
        private static bool _educFormsVisibility;
        
        [ObservableProperty]
        private static bool _coursesVisibility;

        [ObservableProperty]
        private static List<string> _scheduleSourceSource = null!;

        [ObservableProperty]
        private static List<KeyValuePair<string, string>>? _facultiesSource;
        
        [ObservableProperty]
        private static List<KeyValuePair<string, string>>? _educFormsSource;
        
        [ObservableProperty]
        private static List<KeyValuePair<string, string>>? _coursesSource;

        [ObservableProperty]
        private static List<KeyValuePair<string, string>>? _studyGroupsSource;

        [ObservableProperty]
        private string _selectedScheduleSource = Settings.Default.ScheduleSource;

        [ObservableProperty]
        private string _selectedFaculty = Settings.Default.Faculty;
        
        [ObservableProperty]
        private string _selectedEducForm = Settings.Default.EducationForm;
        
        [ObservableProperty]
        private string _selectedCourse = Settings.Default.Course;
        
        [ObservableProperty]
        private string _selectedStudyGroup = Settings.Default.StudyGroup;
        
        [ObservableProperty]
        private bool _showRiggedSchedule = Settings.Default.ShowRiggedSchedule;

        #endregion
        
        #region Methods

        public override void Save()
        {
            var isSettingsChanged = Settings.Default.ShowRiggedSchedule != ShowRiggedSchedule || 
                                    Settings.Default.ScheduleSource != SelectedScheduleSource ||
                                    Settings.Default.StudyGroup != SelectedStudyGroup;

            Settings.Default.ScheduleSource = SelectedScheduleSource;
            Settings.Default.Faculty = SelectedFaculty;
            Settings.Default.EducationForm = SelectedEducForm;
            Settings.Default.Course = SelectedCourse;
            Settings.Default.StudyGroup = SelectedStudyGroup;
            Settings.Default.ShowRiggedSchedule = ShowRiggedSchedule;
            Settings.Default.Save();
            
            if (isSettingsChanged)
            {
                WindowMediator.ReloadRequired();
            }
        }
        
        private async Task InitializeSettings()
        {
            _settingsSource = ScheduleReflector.GetScheduleSettings(SelectedScheduleSource);
            if (_settingsSource == null)
            {
                return;
            }
            
            await _settingsSource.LoadStudentFiltersAsync();
            
            (FacultiesVisibility, EducFormsVisibility, CoursesVisibility) = 
                ScheduleReflector.ConfigureFiltersVisibility(SelectedScheduleSource);
        
            FacultiesSource = _settingsSource.Faculties?.ToList();
            EducFormsSource = _settingsSource.EducationTypes?.ToList();
            CoursesSource = _settingsSource.Courses?.ToList();

            StudyGroupsSource = (await _settingsSource.GetStudyGroupsAsync(
                SelectedFaculty, SelectedCourse, SelectedEducForm))?.ToList();
        }

        private async void SomethingChanged(object? sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(SelectedScheduleSource))
            {
                await InitializeSettings();
            }
            if (args.PropertyName is nameof(SelectedFaculty) or nameof(SelectedCourse) or nameof(SelectedEducForm))
            {
                StudyGroupsSource = (await _settingsSource.GetStudyGroupsAsync(
                    SelectedFaculty, SelectedCourse, SelectedEducForm))?.ToList();
            }
        }

        #endregion
    }
}
