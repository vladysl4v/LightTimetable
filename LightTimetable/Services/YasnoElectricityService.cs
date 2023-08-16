using Newtonsoft.Json;

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using LightTimetable.Models;
using LightTimetable.Properties;
using LightTimetable.Models.Enums;
using LightTimetable.Services.Enums;
using LightTimetable.Services.Models;
using LightTimetable.Handlers.Utilities;
using LightTimetable.Services.Abstractions;

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


namespace LightTimetable.Services
{
    public sealed class YasnoElectricityService : IElectricityService
    {
        private readonly CacheManager<FullElectricityData?> _cache;
        private ElectricityList? _electricityData;

        private readonly IHttpClientFactory _httpFactory;
        private readonly IUserSettings _settings;

        public YasnoElectricityService(IUserSettings settings, IHttpClientFactory httpClientFactory)
        {
            _settings = settings;
            _httpFactory = httpClientFactory;

            _cache = ConfigureCache(settings);
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
                    if (outageInterval["type"] == "POSSIBLE_OUTAGE" && _settings.ShowPossibleOutages)
                        currentOutages.Add(new SpecificOutage(outageTime, OutageType.Possible, dayOfWeek));

                    if (outageInterval["type"] == "DEFINITE_OUTAGE")
                        currentOutages.Add(new SpecificOutage(outageTime, OutageType.Definite, dayOfWeek));
                }
            }

            return currentOutages;
        }

        public async Task InitializeAsync()
        {
            if (_settings.OutagesCity == string.Empty || _settings.OutagesGroup == 0)
                return;

            var outagesCache = _cache.GetCache();

            if (outagesCache != null)
            {
                _electricityData = outagesCache[_settings.OutagesCity]["group_" + _settings.OutagesGroup];
                return;
            }

            string url = "https://kyiv.yasno.com.ua/schedule-turn-off-electricity";

            var httpClient = _httpFactory.CreateClient();
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
            var serializedData = "{" + Regex.Match(request, "\"" + _settings.OutagesCity + "\":{.*?}]]}").Value + "}";

            var cityElectricityData = JsonConvert.DeserializeObject<FullElectricityData>(serializedData);

            if (cityElectricityData == null || !cityElectricityData.ContainsKey(_settings.OutagesCity))
            {
                _electricityData = null;
                return;
            }

            _cache.SetCache(cityElectricityData);

            _electricityData = cityElectricityData[_settings.OutagesCity]["group_" + _settings.OutagesGroup];
        }

        private CacheManager<FullElectricityData?> ConfigureCache(IUserSettings settings)
        {
            return new CacheManager<FullElectricityData?>("yasno-electricity-service", TimeSpan.FromHours(4))
            {
                ExtraDataCondition = (data) => (string)data["city"] != settings.OutagesCity,
                ExtraData = new Dictionary<string, object>
                {
                    { "city", settings.OutagesCity }
                }
            };
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
