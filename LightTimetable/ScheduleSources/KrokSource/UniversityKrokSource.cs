using Newtonsoft.Json;

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Models;
using LightTimetable.Properties;
using LightTimetable.ScheduleSources.Exceptions;
using LightTimetable.ScheduleSources.Abstractions;


namespace LightTimetable.ScheduleSources.KrokSource
{
    public sealed class UniversityKrokSource : IScheduleSource
    {

        private readonly DataItemBuilder _builder;

        private readonly IHttpClientFactory _httpFactory;
        private readonly IUserSettings _settings;

        public UniversityKrokSource(IUserSettings settings,
                                    IHttpClientFactory httpClientFactory,
                                    DataItemBuilder dataItemBuilder)
        {
            _settings = settings;
            _httpFactory = httpClientFactory;

            _builder = dataItemBuilder;
        }

        public async Task<Dictionary<DateTime, List<DataItem>>> LoadDataAsync(DateTime startDate, DateTime endDate)
        {
            var url = $"https://vnz.osvita.net/BetaSchedule.asmx/GetScheduleDataX?" +
                      $"aVuzID=11784&" +
                      $"aStudyGroupID=\"{_settings.StudyGroup}\"&" +
                      $"aStartDate=\"{startDate.ToShortDateString()}\"&" +
                      $"aEndDate=\"{endDate.ToShortDateString()}\"&" +
                      $"aStudyTypeID=null";

            var httpClient = _httpFactory.CreateClient();
            string serializedData;
            try
            {
                serializedData = await httpClient.GetStringAsync(url);
            }
            catch (Exception ex)
            {
                throw new ScheduleLoadingException(ex, _settings.StudyGroup, "Could not load data from vnz osvita.");
            }

            return DeserializeData(serializedData);
        }

        private Dictionary<DateTime, List<DataItem>> DeserializeData(string serializedData)
        {
            var rawDataItems = JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, string>>>>(serializedData);

            if (rawDataItems == null)
            {
                throw new ScheduleLoadingException(_settings.StudyGroup);
            }

            var groupedData = rawDataItems["d"].GroupBy(x => x["full_date"]);

            var result = new Dictionary<DateTime, List<DataItem>>();

            foreach (var group in groupedData)
            {
                result.Add(Convert.ToDateTime(group.Key), new List<DataItem>());

                foreach (var lesson in group)
                {
                    var timePeriod = new TimeInterval(TimeOnly.Parse(lesson["study_time_begin"]),
                            TimeOnly.Parse(lesson["study_time_end"]));

                    _builder.AddTimePeriod(Convert.ToDateTime(lesson["full_date"]), timePeriod);
                    _builder.AddBasicInformation(lesson["discipline"], lesson["study_type"],
                            lesson["cabinet"], lesson["employee"]);

                    if (!string.IsNullOrEmpty(lesson["study_subgroup"]))
                    {
                        _builder.AddPromt(lesson["study_subgroup"]);
                    }

                    var dataItem = _builder.Build();

                    result[Convert.ToDateTime(group.Key)].Add(dataItem);
                }
            }

            return result;
        }
    }
}
