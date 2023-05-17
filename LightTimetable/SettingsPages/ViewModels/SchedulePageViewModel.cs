using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

using System.Collections.Generic;

using LightTimetable.Tools;
using LightTimetable.Views;
using LightTimetable.Properties;


namespace LightTimetable.SettingsPages.ViewModels
{
    public partial class SchedulePageViewModel : PageViewModelBase
    {
        public SchedulePageViewModel(bool disableInitializing) { }
        public SchedulePageViewModel()
        {
            CollectOptions();
            TryRefreshStudyGroups();
            IsAnythingChanged = false;
        }

        #region Properties

        [ObservableProperty]
        private bool _showRiggedSchedule = Settings.Default.ShowRiggedSchedule;

        [ObservableProperty]
        private JArray _facultiesSource;
        
        [ObservableProperty]
        private JArray _educFormsSource;
        
        [ObservableProperty]
        private JArray _coursesSource;
        
        [ObservableProperty]
        private JArray _studyGroupsSource;

        private string _currFaculty = Settings.Default.Faculty;
        private string _currEducForm = Settings.Default.EducationForm;
        private string _currCourse = Settings.Default.Course;
        private string _currStudyGroup = Settings.Default.StudyGroup;

        public JValue CurrentFaculty
        {
            get => new JValue(_currFaculty);
            set
            {
                SetProperty(ref _currFaculty, value.ToString());
                TryRefreshStudyGroups();
            }
        }

        public JValue CurrentEducForm
        {
            get => new JValue(_currEducForm);
            set
            {
                SetProperty(ref _currEducForm, value.ToString());
                TryRefreshStudyGroups();
            }
        }

        public JValue CurrentCourse
        {
            get => new JValue(_currCourse);
            set
            {
                SetProperty(ref _currCourse, value.ToString());
                TryRefreshStudyGroups();
            }
        }

        public JValue CurrentStudyGroup
        {

            get => new JValue(_currStudyGroup);
            set => SetProperty(ref _currStudyGroup, value.ToString());
        }

        #endregion
        
        #region Methods

        public override void Save()
        {
            if (Settings.Default.ShowRiggedSchedule != ShowRiggedSchedule ||
                Settings.Default.StudyGroup != _currStudyGroup)
            {
                SettingsView.IsRequiredReload = true;
            }

            Settings.Default.Faculty = _currFaculty;
            Settings.Default.EducationForm = _currEducForm;
            Settings.Default.Course = _currCourse;
            Settings.Default.StudyGroup = _currStudyGroup;
            Settings.Default.ShowRiggedSchedule = ShowRiggedSchedule;
            Settings.Default.Save();
            IsAnythingChanged = false;
        }

        private async void CollectOptions()
        {
            var url = "https://vnz.osvita.net/BetaSchedule.asmx/GetStudentScheduleFiltersData?&aVuzID=11784";
            var request = await HttpRequestService.LoadStringAsync(url);

            if (request == string.Empty)
            {
                FacultiesSource = new JArray();
                EducFormsSource = new JArray();
                CoursesSource = new JArray();
            }
            else
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, dynamic>>>(request)["d"];

                FacultiesSource = data["faculties"];
                EducFormsSource = data["educForms"];
                CoursesSource = data["courses"];
            }

            IsAnythingChanged = false;
        }

        private void TryRefreshStudyGroups()
        {
            if (_currFaculty != "" && _currCourse != "" && _currEducForm != "")
                CollectGroups();
        }
        private async void CollectGroups()
        {
            var url = $"https://vnz.osvita.net/BetaSchedule.asmx/GetStudyGroups?&aVuzID=11784&aFacultyID=\"{_currFaculty}\"&aEducationForm=\"{_currEducForm}\"&aCourse=\"{_currCourse}\"&aGiveStudyTimes=false";
            var request = await HttpRequestService.LoadStringAsync(url, attemps: 1);

            if (request == string.Empty)
            {
                StudyGroupsSource = new JArray();
            }
            else
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, dynamic>>>(request)["d"];
                StudyGroupsSource = data["studyGroups"];
            }

            IsAnythingChanged = false;
        }

        #endregion
    }
}
