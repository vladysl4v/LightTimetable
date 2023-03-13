using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Net.Http;
using System.Collections.Generic;

using static LightTimetable.Properties.Settings;


namespace LightTimetable.SettingsPages.ViewModels
{
    public class SchedulePageViewModel : PageViewModelBase
    {
        public SchedulePageViewModel()
        {
            CollectOptions();
            TryRefreshStudyGroups();
            IsAnythingChanged = false;
        }

        public override void Save()
        {
            Default.Faculty = _currFaculty;
            Default.EducationForm = _currEducForm;
            Default.Course = _currCourse;
            Default.StudyGroup = _currStudyGroup;
            Default.ShowBlackouts = ShowBlackouts;
            Default.ShowPossibleBlackouts = ShowPossibleBlackouts;
            Default.DTEKGroup = BlackoutsGroup;
            Default.Save();
            IsAnythingChanged = false;
        }

        #region Properties

        private string _currFaculty = Default.Faculty;
        private string _currEducForm = Default.EducationForm;
        private string _currCourse = Default.Course;
        private string _currStudyGroup = Default.StudyGroup;
        private string _blackoutsGroup = Default.DTEKGroup;
        private bool _showBlackouts = Default.ShowBlackouts;
        private bool _showPossibleBlackouts = Default.ShowPossibleBlackouts;

        public bool ShowBlackouts
        {
            get => _showBlackouts;
            set => SetProperty(ref _showBlackouts, value);
        }

        public bool ShowPossibleBlackouts
        {
            get => _showPossibleBlackouts;
            set => SetProperty(ref _showPossibleBlackouts, value);
        }

        public string BlackoutsGroup
        {
            get => _blackoutsGroup;
            set => SetProperty(ref _blackoutsGroup, value);
        }

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

        #region ComboBox sources

        private JArray _facultiesSource;
        private JArray _educFormsSource;
        private JArray _coursesSource;
        private JArray _studyGroupsSource;


        public JArray FacultiesSource
        {
            get => _facultiesSource;
            set => SetProperty(ref _facultiesSource, value);
        }
        public JArray EducFormsSource
        {
            get => _educFormsSource;
            set => SetProperty(ref _educFormsSource, value);
        }
        public JArray CoursesSource
        {
            get => _coursesSource;
            set => SetProperty(ref _coursesSource, value);
        }
        public JArray StudyGroupsSource
        {
            get => _studyGroupsSource;
            set => SetProperty(ref _studyGroupsSource, value);
        }
        #endregion

        #region Methods

        private async void CollectOptions()
        {
            using HttpClient httpClient = new HttpClient();

            string url = "https://vnz.osvita.net/BetaSchedule.asmx/GetStudentScheduleFiltersData?&aVuzID=11784";
            string request = await httpClient.GetStringAsync(url);

            var data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, dynamic>>>(request)["d"];

            FacultiesSource = data["faculties"];
            EducFormsSource = data["educForms"];
            CoursesSource = data["courses"];
            IsAnythingChanged = false;
        }

        private void TryRefreshStudyGroups()
        {
            if (Default.Faculty != null && Default.Course != null && Default.EducationForm != null)
                CollectGroups();
        }
        private async void CollectGroups()
        {
            using HttpClient httpClient = new HttpClient();

            string url = $"https://vnz.osvita.net/BetaSchedule.asmx/GetStudyGroups?&aVuzID=11784&aFacultyID=\"{Default.Faculty}\"&aEducationForm=\"{Default.EducationForm}\"&aCourse=\"{Default.Course}\"&aGiveStudyTimes=false";
            string request = await httpClient.GetStringAsync(url);

            var data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, dynamic>>>(request)["d"];

            StudyGroupsSource = data["studyGroups"];
        }

        #endregion
    }
}
