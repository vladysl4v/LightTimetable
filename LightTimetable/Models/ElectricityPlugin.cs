using Newtonsoft.Json;

using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using LightTimetable.Tools;
using LightTimetable.Properties;
using LightTimetable.Models.Utilities;

using ElectricityDictionary =
    System.Collections.Generic.Dictionary<string,
        System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>>>;


namespace LightTimetable.Models
{
    public static class ElectricityPlugin
    {
        private static ElectricityDictionary _blackoutsData;

        public static ElecticityStatus? GetLightInformation(TimeInterval studyTime, NormalDayOfWeek dayOfWeek)
        {
            var intersections = FindIntersections(studyTime, dayOfWeek);

            if (!Settings.Default.ShowBlackouts || !intersections[1].Any() && !intersections[0].Any())
                return null;

            var isDefinitelyBlackout = false;

            var result = new StringBuilder();
            result.Append("Відключення світла:");
            if (intersections[1].Any())
            {
                int startHour = int.Parse(intersections[1].First()) - 1;
                result.Append($"\n{startHour}:00-{intersections[1].Last()}:00 - електроенергії не буде");
                isDefinitelyBlackout = true;
            }
            if (intersections[0].Any() && Settings.Default.ShowPossibleBlackouts)
            {
                int startHour = int.Parse(intersections[0].First()) - 1;
                result.Append($"\n{startHour}:00-{intersections[0].Last()}:00 - можливе відключення");
            }

            if (!isDefinitelyBlackout && !Settings.Default.ShowPossibleBlackouts)
                return null;

            return new ElecticityStatus(result.ToString(), isDefinitelyBlackout);
        }

        private static string[][] FindIntersections(TimeInterval studyTime, NormalDayOfWeek dayOfWeek)
        {
            if (!Settings.Default.ShowBlackouts)
                return Array.Empty<string[]>();

            var intDayOfWeek = (int)dayOfWeek + 1;

            var currBlackouts = _blackoutsData[intDayOfWeek.ToString()];

            string[] blackoutTimes = currBlackouts["no"].Concat(currBlackouts["maybe"]).ToArray();

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

        #region Data initialization

        public static async Task InitializeBlackoutsAsync()
        {
            if (!Settings.Default.ShowBlackouts || Settings.Default.DTEKGroup == "0")
            {
                _blackoutsData = ConvertToCollection("");
                return;
            }

            var request = await HttpRequestService.LoadStringAsync("https://www.dtek-kem.com.ua/ua/shutdowns");

            if (!request.IsSuccessful)
            {
                _blackoutsData = ConvertToCollection("");
                return;
            }

            var serializedData = Regex.Match(request.Response, "\"data\":{.*").Value[7..^1];

            _blackoutsData = ConvertToCollection(serializedData);
        }

        private static ElectricityDictionary ConvertToCollection(string serializedData)
        {
            ElectricityDictionary tempDictionary = new();

            if (serializedData is "" or null)
            {
                return tempDictionary;
            }

            var currGroup = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(serializedData);

            if (currGroup == null || !currGroup.TryGetValue(Settings.Default.DTEKGroup, out var userElectricity))
            {
                return tempDictionary;
            }

            foreach (var group in userElectricity)
            {
                tempDictionary.Add(group.Key, group.Value.GroupBy(r => r.Value).ToDictionary(t => t.Key, t => t.Select(r => r.Key).ToList()));
            }
            return tempDictionary;
        }

        #endregion
    }
}
