using Newtonsoft.Json.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Tools;
using LightTimetable.Properties;


namespace LightTimetable.SettingsPages.ViewModels
{
    public partial class SchedulePageViewModel : PageViewModelBase
    {
        public SchedulePageViewModel(bool disableInitialation) { }
        public SchedulePageViewModel()
        {
            PropertyChanged += SomethingChanged; 
            Task.Run(async () => 
            {
                await LoadStudentFiltersAsync();
                await LoadStudyGroupsAsync();
            }).ConfigureAwait(false);
        }

        #region Properties

        [ObservableProperty]
        private static List<KeyValuePair<string, string>>? _facultiesSource;
        
        [ObservableProperty]
        private static List<KeyValuePair<string, string>>? _educFormsSource;
        
        [ObservableProperty]
        private static List<KeyValuePair<string, string>>? _coursesSource;

        [ObservableProperty]
        private static List<KeyValuePair<string, string>>? _studyGroupsSource;

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
                                    Settings.Default.StudyGroup != SelectedStudyGroup;

            Settings.Default.Faculty = SelectedFaculty;
            Settings.Default.EducationForm = SelectedEducForm;
            Settings.Default.Course = SelectedCourse;
            Settings.Default.StudyGroup = SelectedStudyGroup;
            Settings.Default.ShowRiggedSchedule = ShowRiggedSchedule;
            
            if (isSettingsChanged)
            {
                WindowMediator.ReloadRequired();
            }
        }

        private async Task LoadStudentFiltersAsync()
        {
            var url = "https://vnz.osvita.net/BetaSchedule.asmx/GetStudentScheduleFiltersData?&" +
                      "aVuzID=11784";

            var serializedData = await HttpRequestService.LoadStringAsync(url);

            if (serializedData == null || serializedData == string.Empty)
                return;
            
            var jsonData = JObject.Parse(serializedData)["d"];

            if (jsonData == null)
                return;
            
            FacultiesSource = ((JArray)jsonData["faculties"]!).ToObject<List<KeyValuePair<string, string>>>();
            EducFormsSource = ((JArray)jsonData["educForms"]!).ToObject<List<KeyValuePair<string, string>>>();
            CoursesSource = ((JArray)jsonData["courses"]!).ToObject<List<KeyValuePair<string, string>>>();

            IsAnythingChanged = false;
        }

        private async Task LoadStudyGroupsAsync()
        {
            if (SelectedCourse == "" || SelectedEducForm == "" || SelectedFaculty == "")
                return;

            var url = $"https://vnz.osvita.net/BetaSchedule.asmx/GetStudyGroups?&" + 
                      $"aVuzID=11784&" + 
                      $"aFacultyID=\"{SelectedFaculty}\"&" + 
                      $"aEducationForm=\"{SelectedEducForm}\"&" + 
                      $"aCourse=\"{SelectedCourse}\"&" + 
                      $"aGiveStudyTimes=false";

            var serializedData = await HttpRequestService.LoadStringAsync(url, maxAttemps: 2);

            if (serializedData == null || serializedData == string.Empty)
                return;

            var jsonData = JObject.Parse(serializedData)["d"];

            if (jsonData == null)
                return;

            StudyGroupsSource = ((JArray)jsonData["studyGroups"]!).ToObject<List<KeyValuePair<string, string>>>();
        }

        private async void SomethingChanged(object? sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName is nameof(SelectedFaculty) or nameof(SelectedCourse) or nameof(SelectedEducForm))
            {
                await LoadStudyGroupsAsync();
            }
        }

        #endregion
    }
}
