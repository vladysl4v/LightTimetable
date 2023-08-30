using Newtonsoft.Json;

using System;
using System.Linq;
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

using OutagesData =
    System.Collections.Generic.Dictionary<int,
        System.Collections.Generic.Dictionary<LightTimetable.Models.Enums.NormalDayOfWeek,
            System.Collections.Generic.List<LightTimetable.Services.Models.SpecificOutage>>>;


namespace LightTimetable.Services
{
    public class DtekElectricityService : IElectricityService
    {
        private readonly CacheManager<OutagesData?> _cache;
        private readonly IHttpClientFactory _httpFactory;
        private readonly IUserSettings _settings;

        private OutagesData? _outages;

        public DtekElectricityService(IUserSettings settings, IHttpClientFactory httpFactory)
        {
            _settings = settings;
            _httpFactory = httpFactory;

            _cache = new CacheManager<OutagesData?>("dtek-electricity-service", TimeSpan.FromHours(12));
        }
        public List<SpecificOutage> GetOutagesInformation(TimeInterval time, NormalDayOfWeek dayOfWeek)
        {
            if (_outages == null)
            {
                return new List<SpecificOutage>();
            }
            var output = _outages[_settings.OutagesGroup][dayOfWeek].Where(x => IsIntervalsIntersects(time, x.Time));
            if (!_settings.ShowPossibleOutages)
            {
                output = output.Where(x => x.Type == OutageType.Definite);
            }

            return output.ToList();
        }

        public async Task InitializeAsync()
        {
            if (_settings.OutagesGroup == 0)
                return;

            var outagesCache = _cache.GetCache();

            if (outagesCache != null)
            {
                _outages = outagesCache;
                return;
            }

            string source = "https://www.dtek-kem.com.ua/ua/shutdowns";
            var httpClient = _httpFactory.CreateClient();
            try
            {
                var request = await httpClient.GetStringAsync(source);
                var serializedData = Regex.Match(request, "\"data\":{.*").Value[7..^1];
                _outages = DeserializeObject(serializedData);
                _cache.SetCache(_outages);
            }
            catch
            {
                _outages = null;
            }
        }

        private OutagesData? DeserializeObject(string serializedData)
        {
            var allGroups =
                JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<SpecificOutage>>>>(serializedData,
                    new OutageFactory());

            return allGroups?.ToDictionary(group => int.Parse(group.Key),
                    group => group.Value.ToDictionary(item => ConvertToDayOfWeek(item.Key),
                        item => item.Value.Where(x => x.Type != OutageType.Not).ToList()));
        }

        private NormalDayOfWeek ConvertToDayOfWeek(string value)
        {
            var integer = int.Parse(value) - 1;
            return (NormalDayOfWeek)integer;
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
