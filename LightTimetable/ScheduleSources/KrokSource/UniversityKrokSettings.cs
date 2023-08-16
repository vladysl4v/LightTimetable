using Newtonsoft.Json.Linq;

using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.ScheduleSources.Abstractions;


namespace LightTimetable.ScheduleSources.KrokSource
{
    public sealed class UniversityKrokSettings : IScheduleSettings
    {
        private readonly IHttpClientFactory _httpFactory;
        public Dictionary<string, string>? Faculties { get; set; }
        public Dictionary<string, string>? Courses { get; set; }
        public Dictionary<string, string>? EducationTypes { get; set; }

        public UniversityKrokSettings(IHttpClientFactory httpClientFactory)
        {
            _httpFactory = httpClientFactory;
        }

        public async Task LoadStudentFiltersAsync()
        {
            var url = "https://vnz.osvita.net/BetaSchedule.asmx/GetStudentScheduleFiltersData?&" +
                      "aVuzID=11784";

            var httpClient = _httpFactory.CreateClient();
            string serializedData;
            try
            {
                serializedData = await httpClient.GetStringAsync(url);
            }
            catch
            {
                return;
            }

            var jsonData = JObject.Parse(serializedData)["d"];

            if (jsonData == null)
                return;

            Faculties = ((JArray)jsonData["faculties"]!)
                .ToObject<List<KeyValuePair<string, string>>>()?
                    .ToDictionary(k => k.Value, v => v.Key);

            EducationTypes = ((JArray)jsonData["educForms"]!)
                .ToObject<List<KeyValuePair<string, string>>>()?
                    .ToDictionary(k => k.Value, v => v.Key);

            Courses = ((JArray)jsonData["courses"]!)
                .ToObject<List<KeyValuePair<string, string>>>()?
                    .ToDictionary(k => k.Value, v => v.Key);
        }

        public async Task<Dictionary<string, string>?> GetStudyGroupsAsync(string? faculty, string? course, string? educationType)
        {
            if (faculty == null || course == null || educationType == null)
            {
                return null;
            }
            var url = $"https://vnz.osvita.net/BetaSchedule.asmx/GetStudyGroups?&" +
                      $"aVuzID=11784&" +
                      $"aFacultyID=\"{faculty}\"&" +
                      $"aEducationForm=\"{educationType}\"&" +
                      $"aCourse=\"{course}\"&" +
                      $"aGiveStudyTimes=false";

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
            var jsonData = JObject.Parse(serializedData)["d"];

            if (jsonData == null)
                return null;

            return ((JArray)jsonData["studyGroups"]!)
                .ToObject<List<KeyValuePair<string, string>>>()?
                    .ToDictionary(k => k.Value, v => v.Key);
        }
    }
}