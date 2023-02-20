using Newtonsoft.Json;

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using Timetable.Utilities;
using System.Text.RegularExpressions;

namespace Timetable
{
    /// <summary>
    /// User information keeper
    /// </summary>
    static public class UserData
    {
        static public string GroupID { get; private set; } = string.Empty;
        static public Dictionary<DateTime, List<Lesson>> Content { get; private set; } = new();
        static public List<DateTime> ContentDates { get; private set; } = new List<DateTime>();
        static public DateTime Date { get; set; }
        static public Dictionary<string, Dictionary<string, List<string>>> GroupedLights { get; set; } = new();
        static private List<Dictionary<string, string>> _content;

        static public async Task Initialize()
        {
            InitializeSettings();
            if (Properties.Settings.Default.ShowBlackouts)
                await InitializeBlackouts();
            await CollectContent();
            FillContentDates();
            InitializeDate();
        }

        static public void FillContentDates()
        {
            ContentDates.Clear();
            ContentDates = Content.Keys.ToList();
        }

        static public void ChangeDate(int amount)
        {
            // If we keep going through the empty content list
            if (!ContentDates.Contains(Date))
            {
                Date = Date.AddDays(amount);
                return;
            }
            int next_index = (ContentDates.IndexOf(Date)) + amount;
            // If we hit the borders, just keep moving into the void
            if (next_index >= ContentDates.Count || next_index < 0)
                Date = Date.AddDays(amount);
            else // Best result, we got the next date
                Date = ContentDates[next_index];
        }

        static public async Task CollectContent()
        {
            string startDate = DateTime.Now.AddDays(-14).ToShortDateString();
            string endDate = DateTime.Now.AddDays(+14).ToShortDateString();

            HttpClient httpClient = new HttpClient();
            string request;
            string sURL = $"https://vnz.osvita.net/BetaSchedule.asmx/GetScheduleDataX?&aVuzID=11784&aStudyGroupID=%22{GroupID}%22&aStartDate=%22{startDate}%22&aEndDate=%22{endDate}%22&aStudyTypeID=null";
            try
            {
                request = await httpClient.GetStringAsync(sURL);
            }
            catch (HttpRequestException)
            {
                request = "";
            }
            _content = JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, string>>>>(request)["d"];
            RestructurizeContent();
        }

        static public void RestructurizeContent()
        {
            Content.Clear();
            foreach (var pair in _content)
            {
                if (Content.ContainsKey(Convert.ToDateTime(pair["full_date"])))
                {
                    Content[Convert.ToDateTime(pair["full_date"])].Add(new Lesson(pair));
                }
                else
                {
                    Content[Convert.ToDateTime(pair["full_date"])] = new List<Lesson>() { new Lesson(pair) };
                }
            }
        }

        static private void InitializeSettings()
        {
            GroupID = Properties.Settings.Default.StudyGroup;
        }
        
        static public async Task InitializeBlackouts()
        {
            GroupedLights.Clear();

            HttpClient httpClient = new HttpClient();

            string request = await httpClient.GetStringAsync("https://www.dtek-kem.com.ua/ua/shutdowns");
            string rawInput = Regex.Match(request, "\"data\":{.*").Value[7..^1];

            var LightOffGroups = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(rawInput);

            Dictionary<string, Dictionary<string, string>> tempGroup = LightOffGroups[Properties.Settings.Default.DTEKGroup];

            foreach (var group in tempGroup)
            {
                GroupedLights.Add(group.Key, group.Value.GroupBy(r => r.Value).ToDictionary(t => t.Key, t => t.Select(r => r.Key).ToList()));
            }
        }

        static public void InitializeDate()
        {
            if (ContentDates.Count == 0)
            {
                Date = DateTime.Now;
                return;
            }
            var curr_date = DateTime.Today;
            if (DateTime.Now.Hour > 18)
                curr_date = curr_date.AddDays(1);

            var nextDates = from date in ContentDates where date >= curr_date select date;

            if (nextDates.Any())
            { 
                Date = nextDates.First();
            }
            else
            {
                Date = ContentDates.Last();
            }
        }
    }
}

