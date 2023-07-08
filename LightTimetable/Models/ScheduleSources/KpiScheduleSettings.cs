using Newtonsoft.Json.Linq;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Tools;
using LightTimetable.DataTypes.Interfaces;


namespace LightTimetable.Models.ScheduleSources
{
    public sealed class KpiScheduleSettings : IScheduleSettings
    {
        public Dictionary<string, string>? Faculties { get; set; }        
        
        [HideFilter]
        public Dictionary<string, string>? Courses { get; set; }
        
        [HideFilter]
        public Dictionary<string, string>? EducationTypes { get; set; }

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
            
            var serializedData = await HttpRequestService.LoadStringAsync(url);

            if (serializedData == null || serializedData == string.Empty)
                return null;
            
            var jsonData = JObject.Parse(serializedData);

            if (jsonData == null)
                return null;
            
            return ((JArray)jsonData["data"]!).ToObject<List<Dictionary<string, string>>>();
        }
    }
}   