using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Tools;
using LightTimetable.Common;
using LightTimetable.Properties;
using LightTimetable.Models.Utilities;


namespace LightTimetable.Models.ScheduleSources
{
    [ScheduleSource("Унiверситет \"КРОК\"", typeof(UniversityKrokSettings))]
    public sealed class UniversityKrokSource : IScheduleSource
    {
        private readonly IServiceProvider _serviceProvider;

        public UniversityKrokSource(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public async Task<Dictionary<DateTime, List<DataItem>>?> LoadDataAsync(DateTime startDate, DateTime endDate)
        {
            var url = $"https://vnz.osvita.net/BetaSchedule.asmx/GetScheduleDataX?" +
                      $"aVuzID=11784&" +
                      $"aStudyGroupID=\"{Settings.Default.StudyGroup}\"&" +
                      $"aStartDate=\"{startDate.ToShortDateString()}\"&" +
                      $"aEndDate=\"{endDate.ToShortDateString()}\"&" +
                      $"aStudyTypeID=null";
            
            var httpClient = _serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient();
            string serializedData;
            try
            {
                serializedData = await httpClient.GetStringAsync(url); 
            }
            catch
            {
                return null;
            }
            var request = await httpClient.GetStringAsync(url);

            return DeserializeData(request);
        }

        private Dictionary<DateTime, List<DataItem>>? DeserializeData(string serializedData)
        {
            var rawDataItems = JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, string>>>>(serializedData);
            
            if (rawDataItems == null)
                return null;
            var groupedData = rawDataItems["d"].GroupBy(x => x["full_date"]);

            var result = new Dictionary<DateTime, List<DataItem>>();

            var builder = new DataItemBuilder();

            builder.AddGlobalServices(_serviceProvider);

            foreach (var group in groupedData)
            {
                result.Add(Convert.ToDateTime(group.Key), new List<DataItem>());

                foreach (var lesson in group)
                {
                    var timePeriod = new TimeInterval(TimeOnly.Parse(lesson["study_time_begin"]),
                            TimeOnly.Parse(lesson["study_time_end"]));
                    
                    builder.AddTimePeriod(Convert.ToDateTime(lesson["full_date"]), timePeriod);
                    builder.AddBasicInformation(lesson["discipline"], lesson["study_type"],
                            lesson["cabinet"], lesson["emplyee"]);
                    builder.AddServices();
                    
                    if (!string.IsNullOrEmpty(lesson["study_subgroup"]))
                    {
                        builder.AddPromt(lesson["study_subgroup"]);
                    }
                    
                    var dataItem = builder.Build();
                    
                    result[Convert.ToDateTime(group.Key)].Add(dataItem);
                }
            }

            return result;
        }
    }
}
