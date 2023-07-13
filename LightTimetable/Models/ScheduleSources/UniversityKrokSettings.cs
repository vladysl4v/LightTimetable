using Newtonsoft.Json.Linq;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Tools;
using LightTimetable.Common;


namespace LightTimetable.Models.ScheduleSources
{
    public sealed class UniversityKrokSettings : IScheduleSettings
    {
        public Dictionary<string, string>? Faculties { get; set; }        
        public Dictionary<string, string>? Courses { get; set; }
        public Dictionary<string, string>? EducationTypes { get; set; }

        public async Task LoadStudentFiltersAsync()
        {
            var url = "https://vnz.osvita.net/BetaSchedule.asmx/GetStudentScheduleFiltersData?&" +
                      "aVuzID=11784";
            
            var serializedData = await HttpRequestService.LoadStringAsync(url);

            if (serializedData == null || serializedData == string.Empty)
                return;
            
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

            var serializedData = await HttpRequestService.LoadStringAsync(url, maxAttemps: 2);

            if (serializedData == null || serializedData == string.Empty)
                return null;

            var jsonData = JObject.Parse(serializedData)["d"];

            if (jsonData == null)
                return null;

            return ((JArray)jsonData["studyGroups"]!)
                .ToObject<List<KeyValuePair<string, string>>>()?
                    .ToDictionary(k => k.Value, v => v.Key);       
        }
    }
}