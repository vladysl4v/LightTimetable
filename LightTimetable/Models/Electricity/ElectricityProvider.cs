using Newtonsoft.Json;

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using LightTimetable.Tools;
using static LightTimetable.Properties.Settings;


namespace LightTimetable.Models.Electricity
{
    public class ElectricityProvider
    {
        private static string _serializedData;
        public static bool IsBlackoutsInitialized { get; set; }

        public static event Action BlackoutsInitialized;

        private Dictionary<string, Dictionary<string, List<string>>> _blackoutsData;

        public ElectricityProvider()
        {
            _blackoutsData = GetData();
        }

        public void ReloadData()
        {
            _blackoutsData = GetData();
        }

        public string[][] FindIntersections(Dictionary<string, string> item)
        {
            if (!Default.ShowBlackouts)
                return Array.Empty<string[]>();

            int dayOfWeek = (int)Convert.ToDateTime(item["full_date"]).DayOfWeek;
            dayOfWeek = dayOfWeek == 0 ? 7 : dayOfWeek;

            var currBlackouts = _blackoutsData[dayOfWeek.ToString()];

            string[] blackoutTimes = currBlackouts["no"].Concat(currBlackouts["maybe"]).ToArray();

            var studyTime = new TimeInterval(item["study_time"]);
            int endHour = studyTime.End.Hour;
            if (studyTime.End.Minute == 0)
                endHour -= 1;

            var lessonPeriod = new List<string>();
            for (int i = studyTime.Start.Hour; i <= endHour; i++)
            {
                lessonPeriod.Add(Convert.ToString(i + 1));
            }
            var intersections = blackoutTimes.Intersect(lessonPeriod).OrderBy(int.Parse).Select(Convert.ToString).ToArray();
            string[] maybeLightHours = currBlackouts["maybe"].Intersect(intersections).ToArray();
            string[] noLightHours = currBlackouts["no"].Intersect(intersections).ToArray();
            return new[] { maybeLightHours, noLightHours };
        }

        public static async Task InitializeBlackoutsAsync()
        {
            using HttpClient httpClient = new HttpClient();

            string request = await httpClient.GetStringAsync("https://www.dtek-kem.com.ua/ua/shutdowns");
            _serializedData = Regex.Match(request, "\"data\":{.*").Value[7..^1];
            IsBlackoutsInitialized = true;
            BlackoutsInitialized.Invoke();
        }

        private Dictionary<string, Dictionary<string, List<string>>> GetData()
        {
            Dictionary<string, Dictionary<string, List<string>>> tempDictionary = new();
            if (_serializedData is "" or null)
            {
                return new Dictionary<string, Dictionary<string, List<string>>>();
            }
            var currGroup = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(_serializedData)[Default.DTEKGroup];
            foreach (var group in currGroup)
            {
                tempDictionary.Add(group.Key, group.Value.GroupBy(r => r.Value).ToDictionary(t => t.Key, t => t.Select(r => r.Key).ToList()));
            }
            return tempDictionary;
        }
    }
}
