using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using LightTimetable.Common;
using LightTimetable.Models.Utilities;

using ElectricityList =
    System.Collections.Generic.List<
        System.Collections.Generic.List<
            System.Collections.Generic.Dictionary<string, string>>>;

using FullElectricityData = 
    System.Collections.Generic.Dictionary<string,
        System.Collections.Generic.Dictionary<string, 
            System.Collections.Generic.List<
                System.Collections.Generic.List<
                    System.Collections.Generic.Dictionary<string, string>>>>>;


namespace LightTimetable.Models.Services
{
    public sealed class YasnoElectricityService : IElectricityService
    {
        private readonly CacheManager<FullElectricityData?> _cache;
        private ElectricityList? _electricityData;
        private readonly bool _showPossibleOutages;
        private readonly string _city;
        private readonly int _group;

        private readonly IServiceProvider _serviceProvider;
        
        public YasnoElectricityService(string city, int group, bool showPossibleOutages, IServiceProvider serviceProvider)
        {
            _city = city;
            _group = group;
            _showPossibleOutages = showPossibleOutages;
            _serviceProvider = serviceProvider;
            
            _cache = new CacheManager<FullElectricityData?>("yasno-electricity-service", TimeSpan.FromHours(4))
            {
                ExtraDataCondition = (data) => ((string)data["city"] != _city),
                ExtraData = new Dictionary<string, object>
                {
                    { "city", _city }
                }
            };
        }

        public List<SpecificOutage> GetOutagesInformation(TimeInterval studyTime, NormalDayOfWeek dayOfWeek)
        {
            if (_electricityData == null)
                return new List<SpecificOutage>();

            var currentOutages = new List<SpecificOutage>();

            foreach (var outageInterval in _electricityData[(int)dayOfWeek])
            {
                var outageStart = TimeOnly.ParseExact(outageInterval["start"], "%H");
                
                if (!TimeOnly.TryParseExact(outageInterval["end"], "%H", out var outageEnd))
                {
                    outageEnd = new TimeOnly(23, 59);
                }
                var outageTime = new TimeInterval(outageStart, outageEnd);

                if (IsIntervalsIntersects(outageTime, studyTime))
                {
                    if (outageInterval["type"] == "POSSIBLE_OUTAGE" && _showPossibleOutages)
                        currentOutages.Add(new SpecificOutage(outageTime, OutageType.Possible, dayOfWeek));
                    
                    if (outageInterval["type"] == "DEFINITE_OUTAGE")
                        currentOutages.Add(new SpecificOutage(outageTime, OutageType.Definite, dayOfWeek));
                }
            }

            return currentOutages;
        }

        public async Task InitializeAsync()
        {
            if (_city == string.Empty || _group == 0)
                return;

            var outagesCache = _cache.GetCache();
            
            if (outagesCache != null)
            {
                _electricityData = outagesCache[_city]["group_" + _group];
                return;
            }

            string url = "https://kyiv.yasno.com.ua/schedule-turn-off-electricity";       
            
            var httpClient = _serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient();
            string request;
            try
            {
                request = await httpClient.GetStringAsync(url);
            }
            catch
            {
                _electricityData = null;
                return;
            }
            var serializedData = "{" + Regex.Match(request, "\"" + _city + "\":{.*?}]]}").Value + "}";
                
            var cityElectricityData = JsonConvert.DeserializeObject<FullElectricityData>(serializedData);
                
            if (cityElectricityData == null || !cityElectricityData.ContainsKey(_city))
            {
                _electricityData = null;
                return;
            }

            _cache.SetCache(cityElectricityData);

            _electricityData = cityElectricityData[_city]["group_" + _group];
        }

        private bool IsIntervalsIntersects(TimeInterval firstInterval, TimeInterval secondInterval)
        {
            var isStartIntersects = secondInterval.Start <= firstInterval.Start && firstInterval.Start < secondInterval.End;
            var isEndIntersects = secondInterval.Start < firstInterval.End && firstInterval.End <= secondInterval.End;
            var isInside = firstInterval.Start <= secondInterval.Start && firstInterval.End >= secondInterval.End; 
            return isStartIntersects || isEndIntersects || isInside;
        }
    }
}
