using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Net.Http;
using System.Windows.Controls;
using System.Collections.Generic;


namespace Timetable.Settings.Pages
{
    /// <summary>
    /// Interaction logic for timetable page
    /// </summary>
    public partial class TimetablePage : Page
    {
        public TimetablePage()
        {
            InitializeComponent();
            CollectOptions();
        }

        public async void CollectOptions()
        {
            HttpClient httpClient = new HttpClient();

            string headersURL = "https://vnz.osvita.net/BetaSchedule.asmx/GetStudentScheduleFiltersData?&aVuzID=11784";

            string request = await httpClient.GetStringAsync(headersURL);
            var deserialization = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, dynamic>>>(request);

            var CollectedOptions = deserialization["d"];

            comboFaculties.ItemsSource = CollectedOptions["faculties"];
            comboEducForms.ItemsSource = CollectedOptions["educForms"];
            comboCourses.ItemsSource = CollectedOptions["courses"];

            LoadFromSettings();
        }

        private void LoadFromSettings()
        {
            comboFaculties.SelectedValue = new JValue(Properties.Settings.Default.Faculty);
            comboEducForms.SelectedValue = new JValue(Properties.Settings.Default.EducForm);
            comboCourses.SelectedValue = new JValue(Properties.Settings.Default.Course);
            comboStudyGroups.SelectedValue = new JValue(Properties.Settings.Default.StudyGroup);
        }

        public void comboBox_Selected(object sender, EventArgs e)
        {
            if (comboCourses.SelectedIndex != -1 && comboEducForms.SelectedIndex != -1 && comboFaculties.SelectedIndex != -1)
            {
                CollectGroups();
            }
        }

        public async void CollectGroups()
        {
            HttpClient httpClient = new HttpClient();
            string newURL = $"https://vnz.osvita.net/BetaSchedule.asmx/GetStudyGroups?&aVuzID=11784&aFacultyID=\"{comboFaculties.SelectedValue}\"&aEducationForm=\"{comboEducForms.SelectedValue}\"&aCourse=\"{comboCourses.SelectedValue}\"&aGiveStudyTimes=false";
            string request = await httpClient.GetStringAsync(newURL);
            var deserialization = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, dynamic>>>(request);
            var CollectedOptions = deserialization["d"];
            comboStudyGroups.ItemsSource = CollectedOptions["studyGroups"];
        }
    }

}
