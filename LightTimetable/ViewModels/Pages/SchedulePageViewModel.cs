using CommunityToolkit.Mvvm.ComponentModel;

using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Models;
using LightTimetable.Properties;
using LightTimetable.ScheduleSources;
using LightTimetable.ScheduleSources.Abstractions;


namespace LightTimetable.ViewModels.Pages
{
    public partial class SchedulePageViewModel : PageViewModelBase
    {
        private readonly SchedulesRepository _schedulesRepository;
        private readonly UpdatesMediator _mediator;
        private readonly IUserSettings _settings;

        private IScheduleSettings? _settingsSource;

        public SchedulePageViewModel(IUserSettings settings, UpdatesMediator mediator, SchedulesRepository schedulesRepository)
        {
            _schedulesRepository = schedulesRepository;
            _settings = settings;
            _mediator = mediator;


            _selectedScheduleSource = _settings.ScheduleSource;
            _selectedFaculty = _settings.Faculty;
            _selectedEducForm = _settings.EducationForm;
            _selectedCourse = _settings.Course;
            _selectedStudyGroup = _settings.StudyGroup;
            _showRiggedSchedule = _settings.ShowRiggedSchedule;

            PropertyChanged += SomethingChanged;

            ScheduleSourceSource = _schedulesRepository.GetScheduleNames();
            
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
        private string _selectedScheduleSource;

        [ObservableProperty]
        private string _selectedFaculty;
        
        [ObservableProperty]
        private string _selectedEducForm;
        
        [ObservableProperty]
        private string _selectedCourse;
        
        [ObservableProperty]
        private string _selectedStudyGroup;
        
        [ObservableProperty]
        private bool _showRiggedSchedule;

        #endregion
        
        #region Methods

        public override void Save()
        {
            var isSettingsChanged = _settings.ShowRiggedSchedule != ShowRiggedSchedule ||
                                    _settings.ScheduleSource != SelectedScheduleSource ||
                                    _settings.StudyGroup != SelectedStudyGroup;

            _settings.ScheduleSource = SelectedScheduleSource;
            _settings.Faculty = SelectedFaculty;
            _settings.EducationForm = SelectedEducForm;
            _settings.Course = SelectedCourse;
            _settings.StudyGroup = SelectedStudyGroup;
            _settings.ShowRiggedSchedule = ShowRiggedSchedule;
            _settings.Save();
            
            if (isSettingsChanged)
            {
                _mediator.ReloadRequired();
            }
        }
        
        private async Task InitializeSettings()
        {
            _settingsSource = _schedulesRepository.GetScheduleSettings(_selectedScheduleSource);
            if (_settingsSource == null)
            {
                return;
            }
            
            await _settingsSource.LoadStudentFiltersAsync();
            
            (FacultiesVisibility, EducFormsVisibility, CoursesVisibility) =
                _settingsSource.FiltersVisibility;
        
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
