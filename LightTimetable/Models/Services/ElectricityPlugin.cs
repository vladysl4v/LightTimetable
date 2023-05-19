using Newtonsoft.Json;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using LightTimetable.Tools;
using LightTimetable.Models.Utilities;

using ElectricityList =
    System.Collections.Generic.List<
        System.Collections.Generic.List<
            System.Collections.Generic.Dictionary<string, string>>>;

namespace LightTimetable.Models.Services
{
    public static class ElectricityPlugin
    {   
        private static ElectricityList? _electricityData;
        private static bool _showPossibleOutages;

        public static ElectricityStatus? GetElectricityInformation(TimeInterval studyTime, NormalDayOfWeek dayOfWeek)
        {
            if (_electricityData == null)
                return null;
        
            var currentOutages = new List<(TimeInterval, string)>();

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
                        currentOutages.Add((outageTime, outageInterval["type"]));
                    
                    if (outageInterval["type"] == "DEFINITE_OUTAGE")
                        currentOutages.Add((outageTime, outageInterval["type"]));                    
                }
            }

            return currentOutages.Any() ? new ElectricityStatus(currentOutages) : null;
        }

        private static bool IsIntervalsIntersects(TimeInterval firstInterval, TimeInterval secondInterval)
        {
            var isStartIntersects = secondInterval.Start <= firstInterval.Start && firstInterval.Start < secondInterval.End;
            var isEndIntersects = secondInterval.Start < firstInterval.End && firstInterval.End <= secondInterval.End;
            var isInside = firstInterval.Start <= secondInterval.Start && firstInterval.End >= secondInterval.End; 
            return isStartIntersects || isEndIntersects || isInside;
        }

        public static async Task InitializeOutagesAsync(string city, int groupNumber, bool showPossibleOutages)
        {
            _showPossibleOutages = showPossibleOutages;
            _electricityData = null;

            if (city == string.Empty || groupNumber == 0)
                return;

            var request = await HttpRequestService.LoadStringAsync("https://kyiv.yasno.com.ua/schedule-turn-off-electricity");

            if (request == string.Empty)
                return;

            try 
            {
                var serializedData = "{" + Regex.Match(request, "\"" + city + "\":{.*?}]]}").Value + "}";
                var cityElectricityData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<List<Dictionary<string, string>>>>>>(serializedData);    
                _electricityData = cityElectricityData[city]["group_" + groupNumber];
            }
            catch
            {
                return;
            }
        }
    }
}
