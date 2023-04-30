using Newtonsoft.Json;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Tools;
using LightTimetable.Properties;
using LightTimetable.Models.Utilities;


namespace LightTimetable.Models.ScheduleSources
{
    public class VnzOsvitaSource : BaseScheduleSource
    {
        public override async Task InitializeScheduleAsync(DateTime startDate, DateTime endDate)
        {
            var url = $"https://vnz.osvita.net/BetaSchedule.asmx/GetScheduleDataX?" +
                      $"aVuzID=11784&" +
                      $"aStudyGroupID=\"{Settings.Default.StudyGroup}\"&" +
                      $"aStartDate=\"{startDate.ToShortDateString()}\"&" +
                      $"aEndDate=\"{endDate.ToShortDateString()}\"&" +
                      $"aStudyTypeID=null";

            var request = await HttpRequestService.LoadStringAsync(url);

            ScheduleDictionary = request.IsSuccessful ? DeserializeData(request.Response) : null;

            FinishInitialization();
        }

        private Dictionary<DateTime, List<DataItem>> DeserializeData(string serializedData)
        {
            if (serializedData == "" || serializedData.Length < 15)
            {
                return new Dictionary<DateTime, List<DataItem>>();
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
                    result[Convert.ToDateTime(group.Key)].Add(new DataItem(lesson));
                }
            }

            return result;
        }
    }
}
