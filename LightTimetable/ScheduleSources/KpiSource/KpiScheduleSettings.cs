using Newtonsoft.Json.Linq;

using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.ScheduleSources.Abstractions;


namespace LightTimetable.ScheduleSources.KpiSource
{
    public sealed class KpiScheduleSettings : IScheduleSettings
    {
        private readonly IHttpClientFactory _httpFactory;
        

        public Dictionary<string, string>? Faculties { get; set; }
        public Dictionary<string, string>? Courses { get; set; }
        public Dictionary<string, string>? EducationTypes { get; set; }

        public KpiScheduleSettings(IHttpClientFactory httpClientFactory)
        {
            _httpFactory = httpClientFactory;
        }

        public async Task<Dictionary<string, string>?> GetStudyGroupsAsync(string? faculty, string? course = null, string? studyType = null)
        {
            if (faculty == null)
                return null;

            var scheduleData = await LoadScheduleDataAsync();

            if (scheduleData == null)
                return null;

            var filteredGroups = new Dictionary<string, string>();

            foreach (var group in scheduleData.Where(x => x["faculty"] == faculty))
            {
                filteredGroups[group["name"]] = group["id"];
            }
            return filteredGroups;
        }

        public async Task LoadStudentFiltersAsync()
        {
            var scheduleData = await LoadScheduleDataAsync();

            if (scheduleData == null)
                return;

            Faculties = new Dictionary<string, string>();

            foreach (var group in scheduleData)
            {
                var nameTag = string.IsNullOrEmpty(group["faculty"]) ? "(Пусто)" : group["faculty"];

                Faculties[nameTag] = group["faculty"];
            }
        }

        private async Task<List<Dictionary<string, string>>?> LoadScheduleDataAsync()
        {
            var url = "https://schedule.kpi.ua/api/schedule/groups";

            var httpClient = _httpFactory.CreateClient();
            string serializedData;
            try
            {
                serializedData = await httpClient.GetStringAsync(url);
            }
            catch
            {
                return null;
            }

            var jsonData = JObject.Parse(serializedData);

            return ((JArray)jsonData["data"]!).ToObject<List<Dictionary<string, string>>>();
        }
    }
}