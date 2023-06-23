using Newtonsoft.Json;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Tools;
using LightTimetable.Properties;
using LightTimetable.Models.Services;
using LightTimetable.Models.Utilities;


namespace LightTimetable.Models.ScheduleSources
{
    public class VnzOsvitaSource : IScheduleSource
    {
        private ElectricityService? _electricityService;
        private TeamsEventsService? _teamsService;
        public VnzOsvitaSource(ElectricityService? electricityService, TeamsEventsService? teamsService)
        {
            _electricityService = electricityService;
            _teamsService = teamsService;
        }
        public async Task<Dictionary<DateTime, List<DataItem>>?> LoadDataAsync(DateTime startDate, DateTime endDate)
        {
            var url = $"https://vnz.osvita.net/BetaSchedule.asmx/GetScheduleDataX?" +
                      $"aVuzID=11784&" +
                      $"aStudyGroupID=\"{Settings.Default.StudyGroup}\"&" +
                      $"aStartDate=\"{startDate.ToShortDateString()}\"&" +
                      $"aEndDate=\"{endDate.ToShortDateString()}\"&" +
                      $"aStudyTypeID=null";

            var request = await HttpRequestService.LoadStringAsync(url);

            return DeserializeData(request);
        }

        private Dictionary<DateTime, List<DataItem>>? DeserializeData(string serializedData)
        {
            if (serializedData == string.Empty || serializedData.Length < 15)
            {
                return null;
            }

            var rawDataItems =
                JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, string>>>>(serializedData)["d"]
                    .GroupBy(x => x["full_date"]);

            var result = new Dictionary<DateTime, List<DataItem>>();

            foreach (var group in rawDataItems)
            {
                result.Add(Convert.ToDateTime(group.Key), new List<DataItem>());

                foreach (var lesson in group)
                {
                    var dataItem = new DataItem
                    (
                        Convert.ToDateTime(lesson["full_date"]),
                        new TimeInterval(TimeOnly.Parse(lesson["study_time_begin"]), TimeOnly.Parse(lesson["study_time_end"])),
                        lesson["discipline"],
                        lesson["study_type"],
                        lesson["employee"],
                        lesson["cabinet"],
                        lesson["study_subgroup"],
                        _teamsService,
                        _electricityService
                    );

                    result[Convert.ToDateTime(group.Key)].Add(dataItem);
                }
            }

            return result;
        }
    }
}
