using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Net.Http;
using System.Collections.Generic;

using static LightTimetable.Properties.Settings;


namespace LightTimetable.ViewModels.Pages
{
    public class TimetablePageViewModel : ViewModelBase
    {
        public TimetablePageViewModel()
        {
            CollectOptions();
            TryRefreshStudyGroups();
        }


        #region Properties

        private JArray _allFaculties = null!;
        private JArray _allEducForms = null!;
        private JArray _allCourses = null!;
        private JArray _allStudyGroups = null!;

        public JValue CurrentFaculty
        {
            get => new JValue(Default.Faculty);
            set
            {
                Default.Faculty = value.ToString();
                TryRefreshStudyGroups();
            }
        }
        public JValue CurrentEducForm
        {
            get => new JValue(Default.EducationForm);
            set
            {
                Default.EducationForm = value.ToString();
                TryRefreshStudyGroups();
            }
        }
        public JValue CurrentCourse
        {
            get => new JValue(Default.Course);
            set
            {
                Default.Course = value.ToString();
                TryRefreshStudyGroups();
            }
        }
        public JValue CurrentStudyGroup
        {
            get => new JValue(Default.StudyGroup);
            set
            {
                Default.StudyGroup = value.ToString();
            }
        }

        public JArray AllFaculties
        {
            get => _allFaculties;
            set => SetProperty(ref _allFaculties, value);
        }
        public JArray AllEducForms
        {
            get => _allEducForms;
            set => SetProperty(ref _allEducForms, value);
        }
        public JArray AllCourses
        {
            get => _allCourses;
            set => SetProperty(ref _allCourses, value);
        }
        public JArray AllStudyGroups
        {
            get => _allStudyGroups;
            set => SetProperty(ref _allStudyGroups, value);
        }
        #endregion

        #region Methods

        private async void CollectOptions()
        {
            using HttpClient httpClient = new HttpClient();

            string url = "https://vnz.osvita.net/BetaSchedule.asmx/GetStudentScheduleFiltersData?&aVuzID=11784";
            string request = await httpClient.GetStringAsync(url);

            var data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, dynamic>>>(request)["d"];

            AllFaculties = data["faculties"];
            AllEducForms = data["educForms"];
            AllCourses = data["courses"];
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

            AllStudyGroups = data["studyGroups"];
        }

        #endregion


    }
}
