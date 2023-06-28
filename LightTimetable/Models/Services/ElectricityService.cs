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
    public class ElectricityService
    {
        private const string _outagesUrl = "https://kyiv.yasno.com.ua/schedule-turn-off-electricity"; 
        private ElectricityList? _electricityData;
        private bool _showPossibleOutages;
        private int _groupNumber;
        private string _city;

        public ElectricityService(string city, int groupNumber, bool showPossibleOutages)
        {
            _city = city;
            _groupNumber = groupNumber;
            _showPossibleOutages = showPossibleOutages;
        }

        public ElectricityStatus? GetElectricityInformation(TimeInterval studyTime, NormalDayOfWeek dayOfWeek)
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

        private bool IsIntervalsIntersects(TimeInterval firstInterval, TimeInterval secondInterval)
        {
            var isStartIntersects = secondInterval.Start <= firstInterval.Start && firstInterval.Start < secondInterval.End;
            var isEndIntersects = secondInterval.Start < firstInterval.End && firstInterval.End <= secondInterval.End;
            var isInside = firstInterval.Start <= secondInterval.Start && firstInterval.End >= secondInterval.End; 
            return isStartIntersects || isEndIntersects || isInside;
        }

        public async Task InitializeOutagesAsync()
        {
            if (_city == string.Empty || _groupNumber == 0)
                return;

            var request = await HttpRequestService.LoadStringAsync(_outagesUrl);

            if (request == null)
                return;

            try 
            {
                var serializedData = "{" + Regex.Match(request, "\"" + _city + "\":{.*?}]]}").Value + "}";
                
                var cityElectricityData = 
                    JsonConvert.DeserializeObject<Dictionary<string, Dictionary<
                        string, List<List<Dictionary<string, string>>>>>>(serializedData);
                        
                _electricityData = cityElectricityData[_city]["group_" + _groupNumber];
            }
            catch
            {
                _electricityData = null;
            }
        }
    }
}
