using Newtonsoft.Json.Linq;

using System;
using System.Linq;
using System.Net.Http;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Models;
using LightTimetable.Properties;
using LightTimetable.Models.Enums;
using LightTimetable.Models.Extensions;
using LightTimetable.ScheduleSources.Exceptions;
using LightTimetable.ScheduleSources.Abstractions;

using WeekDictionary =
    System.Collections.Generic.Dictionary<LightTimetable.Models.Enums.NormalDayOfWeek, System.Collections.Generic.List<
        System.Collections.Generic.Dictionary<string, string>>?>;


namespace LightTimetable.ScheduleSources.KpiSource
{
    public sealed class KpiScheduleSource : IScheduleSource
    {
        public string Name => "КПI iм. Iгоря Сiкорського";

        private readonly Calendar _calendar = new GregorianCalendar();
        private readonly DataItemBuilder _builder;

        private readonly IHttpClientFactory _httpFactory;
        private readonly IUserSettings _settings;

        public KpiScheduleSource(IUserSettings settings,
                                 IHttpClientFactory httpClientFactory,
                                 DataItemBuilder dataItemBuilder)
        {
            _settings = settings;
            _httpFactory = httpClientFactory;

            _builder = dataItemBuilder;
        }

        public async Task<Dictionary<DateTime, List<DataItem>>> LoadDataAsync(DateTime startDate, DateTime endDate)
        {
            var outputData = new Dictionary<DateTime, List<DataItem>>();

            var (firstWeek, secWeek) = await LoadScheduleForWeeks(_settings.StudyGroup);

            if (firstWeek == null || secWeek == null)
            {
                throw new ScheduleLoadingException(_settings.StudyGroup, "Schedule for weeks has not been loaded.");
            }

            for (var date = startDate; date < endDate; date = date.AddDays(1))
            {
                var dataItems = new List<DataItem>();

                var properWeek = IsWeekPrimary(date) ? firstWeek : secWeek;

                if (!properWeek.ContainsKey(date.GetNormalDayOfWeek()))
                    continue;

                foreach (var lesson in properWeek[date.GetNormalDayOfWeek()])
                {
                    var time = TimeOnly.ParseExact(lesson["time"], "H.mm");

                    var place = lesson["place"];
                    if (place.LastOrDefault() == '-')
                    {
                        place = place[..^1];
                    }
                    _builder.AddTimePeriod(date, time, new TimeSpan(1, 35, 0));

                    _builder.AddBasicInformation(lesson["name"], lesson["type"], place, lesson["teacherName"]);

                    dataItems.Add(_builder.Build());
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

            var httpClient = _httpFactory.CreateClient();

            string serializedData;
            try
            {
                serializedData = await httpClient.GetStringAsync(url);
            }
            catch (Exception ex)
            {
                throw new ScheduleLoadingException(ex, _settings.StudyGroup, "Could not load data from KPI API.");
            }

            var jsonData = JObject.Parse(serializedData);

            if (!jsonData.ContainsKey("data"))
                return (null, null);

            var scheduleFirstData = ConvertToWeekDictionary(jsonData["data"]["scheduleFirstWeek"]);
            var scheduleSecData = ConvertToWeekDictionary(jsonData["data"]["scheduleSecondWeek"]);

            return (scheduleFirstData, scheduleSecData);
        }

        private bool IsWeekPrimary(DateTime date)
        {
            var intPrimariness = _calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday) % 2;
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