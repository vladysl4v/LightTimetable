using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Collections.Generic;

using LightTimetable.Tools;
using LightTimetable.Properties;


namespace LightTimetable.SettingsPages.ViewModels
{
    public class SchedulePageViewModel : PageViewModelBase
    {
        public SchedulePageViewModel(bool disableInitializing = false) { }
        public SchedulePageViewModel()
        {
            CollectOptions();
            TryRefreshStudyGroups();
            IsAnythingChanged = false;
        }

        #region Properties

        private string _currFaculty = Settings.Default.Faculty;
        private string _currEducForm = Settings.Default.EducationForm;
        private string _currCourse = Settings.Default.Course;
        private string _currStudyGroup = Settings.Default.StudyGroup;
        private string _blackoutsGroup = Settings.Default.DTEKGroup;
        private bool _showBlackouts = Settings.Default.ShowBlackouts;
        private bool _showPossibleBlackouts = Settings.Default.ShowPossibleBlackouts;
        private bool _showRiggedSchedule = Settings.Default.ShowRiggedSchedule;

        public bool ShowBlackouts
        {
            get => _showBlackouts;
            set => SetProperty(ref _showBlackouts, value);
        }

        public bool ShowRiggedSchedule
        {
            get => _showRiggedSchedule;
            set => SetProperty(ref _showRiggedSchedule, value);
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

        public override void Save()
        {
            Settings.Default.Faculty = _currFaculty;
            Settings.Default.EducationForm = _currEducForm;
            Settings.Default.Course = _currCourse;
            Settings.Default.StudyGroup = _currStudyGroup;
            Settings.Default.ShowBlackouts = ShowBlackouts;
            Settings.Default.ShowPossibleBlackouts = ShowPossibleBlackouts;
            Settings.Default.DTEKGroup = BlackoutsGroup;
            Settings.Default.ShowRiggedSchedule = ShowRiggedSchedule;
            Settings.Default.Save();
            IsAnythingChanged = false;
        }

        private async void CollectOptions()
        {
            var url = "https://vnz.osvita.net/BetaSchedule.asmx/GetStudentScheduleFiltersData?&aVuzID=11784";
            var request = await HttpRequestService.LoadStringAsync(url);

            if (!request.IsSuccessful)
            {
                FacultiesSource = new JArray();
                EducFormsSource = new JArray();
                CoursesSource = new JArray();
            }
            else
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, dynamic>>>(request.Response)["d"];

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

            if (!request.IsSuccessful)
            {
                StudyGroupsSource = new JArray();
            }
            else
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, dynamic>>>(request.Response)["d"];
                StudyGroupsSource = data["studyGroups"];
            }

            IsAnythingChanged = false;
        }

        #endregion
    }
}
