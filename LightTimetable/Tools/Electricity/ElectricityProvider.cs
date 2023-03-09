using Newtonsoft.Json;

using System;
using System.Text;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using static LightTimetable.Properties.Settings;


namespace LightTimetable.Tools.Electricity
{
    public class ElectricityProvider
    {
        private static Dictionary<string, Dictionary<string, List<string>>> _groupedLights;

        public ElecticityStatus? GetLightInformation(TimeInterval studyTime, string date)
        {
            if (!Default.ShowBlackouts)
                return null;
            int dayOfWeek = (int)Convert.ToDateTime(date).DayOfWeek;
            dayOfWeek = (dayOfWeek == 0) ? 7 : dayOfWeek;
            var currBlackouts = _groupedLights[dayOfWeek.ToString()];

            string[] intersections = FindIntersections(studyTime, currBlackouts);

            if (intersections.Length == 0)
                return null;
            string toolTipString = CreateToolTipString(intersections, currBlackouts, out bool isDefBlackout);
            if (!Default.ShowPossibleBlackouts && !isDefBlackout)
                return null;
            return new ElecticityStatus(toolTipString, isDefBlackout);
        }

        private string CreateToolTipString(string[] intersections, Dictionary<string, List<string>> currBlackouts, out bool isDefinitelyBlackout)
        {
            string[] noLightHours = currBlackouts["no"].Intersect(intersections).ToArray();
            string[] maybeLightHours = currBlackouts["maybe"].Intersect(intersections).ToArray();

            isDefinitelyBlackout = false;

            StringBuilder result = new StringBuilder();
            result.Append("Ймовірні відключення:");
            if (noLightHours.Any())
            {
                int startHour = int.Parse(noLightHours.First()) - 1;
                result.Append($"\n{startHour}:00-{noLightHours.Last()}:00 - електроенергії не буде");
                isDefinitelyBlackout = true;
            }
            else if (maybeLightHours.Any() && Default.ShowPossibleBlackouts)
            {
                int startHour = int.Parse(maybeLightHours.First()) - 1;
                result.Append($"\n{startHour}:00-{maybeLightHours.Last()}:00 - можливе відключення");
            }
            return result.ToString();
        }

        private string[] FindIntersections(TimeInterval studyTime, Dictionary<string, List<string>> currBlackouts)
        {

            string[] CanBeNoLight = currBlackouts["no"].Concat(currBlackouts["maybe"]).ToArray();
            int endHour = studyTime.End.Hour;
            if (studyTime.End.Minute == 0)
                endHour -= 1;

            List<string> lessonPeriod = new List<string>();
            for (int i = studyTime.Start.Hour; i <= endHour; i++)
            {
                lessonPeriod.Add(Convert.ToString(i + 1));
            }
            return CanBeNoLight.Intersect(lessonPeriod).OrderBy(int.Parse).Select(Convert.ToString).ToArray();
        }
        public static async Task InitializeBlackoutsAsync()
        {
            _groupedLights = new();
            using HttpClient httpClient = new HttpClient();

            string request = await httpClient.GetStringAsync("https://www.dtek-kem.com.ua/ua/shutdowns");
            string rawInput = Regex.Match(request, "\"data\":{.*").Value[7..^1];

            var LightOffGroups = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(rawInput)[Default.DTEKGroup];

            foreach (var group in LightOffGroups)
            {
                _groupedLights.Add(group.Key, group.Value.GroupBy(r => r.Value).ToDictionary(t => t.Key, t => t.Select(r => r.Key).ToList()));
            }
        }
    }
}
