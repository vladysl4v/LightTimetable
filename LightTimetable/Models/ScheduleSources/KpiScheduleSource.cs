using Newtonsoft.Json.Linq;

using System;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Tools;
using LightTimetable.Properties;
using LightTimetable.DataTypes.Enums;
using LightTimetable.Models.Services;
using LightTimetable.Models.Utilities;
using LightTimetable.DataTypes.Interfaces;

using WeekDictionary = System.Collections.Generic.
    Dictionary<LightTimetable.DataTypes.Enums.NormalDayOfWeek,
                    System.Collections.Generic.List<System.Collections
                        .Generic.Dictionary<string, string>>?>;


namespace LightTimetable.Models.ScheduleSources
{
    [ScheduleSource("КПI iм. Iгоря Сiкорського", typeof(KpiScheduleSettings))]
    public sealed class KpiScheduleSource : IScheduleSource
    {
        private Calendar _calendar = new GregorianCalendar();
        private ElectricityService? _electricityService;
        private TeamsEventsService? _teamsService;

        public KpiScheduleSource(ElectricityService? electricityService, TeamsEventsService? teamsService)
        {
            _electricityService = electricityService;
            _teamsService = teamsService;
        }

        public async Task<Dictionary<DateTime, List<DataItem>>?> LoadDataAsync(DateTime startDate, DateTime endDate)
        {
            var outputData = new Dictionary<DateTime, List<DataItem>>();

            (var firstWeek, var secWeek) = await LoadScheduleForWeeks(Settings.Default.StudyGroup);

            if (firstWeek == null | secWeek == null)
                return null;           

            for (var date = startDate; date < endDate; date = date.AddDays(1))
            {    
                var dataItems = new List<DataItem>();
                
                var properWeek = IsWeekPrimary(date) == true ? firstWeek : secWeek;

                if (!properWeek.ContainsKey(date.GetNormalDayOfWeek()))
                    continue;

                foreach (var lesson in properWeek[date.GetNormalDayOfWeek()])
                {
                    var time = TimeOnly.ParseExact(lesson["time"], "H.mm");
                    dataItems.Add(new DataItem
                    (
                        date,
                        new TimeInterval(time, time.Add(new TimeSpan(1, 35, 0))),
                        lesson["name"],
                        lesson["type"],
                        lesson["teacherName"],
                        lesson["place"],
                        electricityLoader: _electricityService == null ? null
                        : _electricityService.GetElectricityInformation,
                        teamsEventsLoader: _teamsService == null ? null
                        : _teamsService.GetSuitableEvents  
                    ));
                }
                dataItems.Sort();

                outputData.Add(date, dataItems);
            }
            
            return outputData;
        }

        private async Task<(WeekDictionary?, WeekDictionary?)> LoadScheduleForWeeks(string groupId)
        {
            var url = "https://schedule.kpi.ua/api/schedule/lessons?" + 
                      $"groupId={groupId}";
            
            var serializedData = await HttpRequestService.LoadStringAsync(url);

            if (serializedData == null || serializedData == string.Empty)
                return (null, null);
            
            var jsonData = JObject.Parse(serializedData);

            if (jsonData == null)
                return (null, null);

            var scheduleFirstData = ConvertToWeekDictionary(jsonData["data"]["scheduleFirstWeek"]);
            var scheduleSecData = ConvertToWeekDictionary(jsonData["data"]["scheduleSecondWeek"]);
            
            return (scheduleFirstData, scheduleSecData);
        }

        private bool IsWeekPrimary(DateTime date)
        {
            var intPrimariness = _calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstDay, DayOfWeek.Monday) % 2;
            return intPrimariness == 1;
        }
        
        private WeekDictionary ConvertToWeekDictionary(JToken jsonData)
        {
            var scheduleData = new WeekDictionary();
            var currentDayOfWeek = NormalDayOfWeek.Monday;
            
            foreach (var pair in jsonData)
            {
                
                var pairsList = pair["pairs"].ToObject<List<Dictionary<string, string>>>();
                if (pairsList.Any())
                {
                    scheduleData.Add(currentDayOfWeek, pairsList);
                }
                currentDayOfWeek++;   
            }

            return scheduleData;
        }
    }
}