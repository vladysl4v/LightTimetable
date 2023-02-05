using Newtonsoft.Json;

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Timetable
{
    /// <summary>
    /// User information keeper
    /// </summary>
    static public class UserData
    {
        static public string GroupID { get; private set; } = string.Empty;
        static public List<Dictionary<string, string>?>? Content { get; private set; } = default;
        static public List<DateTime> ContentDates { get; private set; } = new List<DateTime>();
        static public int LessonsCount { get; set; }
        static public DateTime Date { get; set; }

        static public async Task Initialize()
        {
            InitSettings();
            await CollectContent();
            SetContentDates();
            InitDate();
        }

        static private void SetContentDates()
        {
            ContentDates.Clear();
            var temp_dates = (from lesson in Content select lesson["full_date"]).Distinct();
            foreach (var date in temp_dates)
                ContentDates.Add(Convert.ToDateTime(date));
        }

        static private void InitSettings()
        {
            GroupID = Properties.Settings.Default.StudyGroup;
        }

        static public void InitDate()
        {
            if (ContentDates.Count == 0)
            {
                Date = DateTime.Now;
                return;
            }
            var curr_date = DateTime.Today;
            if (DateTime.Now.Hour > 18)
                curr_date = curr_date.AddDays(1);
            try
            {
                Date = (from date in ContentDates where date >= curr_date select date).First();
            }
            catch (System.InvalidOperationException)
            {
                Date = ContentDates.Last();
            }
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
            if (next_index >= ContentDates.Count || next_index < 0 )
                Date = Date.AddDays(amount);
            else // Best result, we got the next date
                Date = ContentDates[next_index];
        }

        static public async Task CollectContent()
        {
            HttpClient httpClient = new HttpClient();

            string StartDate = DateTime.Now.AddDays(-14).ToShortDateString();
            string EndDate = DateTime.Now.AddDays(+14).ToShortDateString();

            string sURL = $"https://vnz.osvita.net/BetaSchedule.asmx/GetScheduleDataX?&aVuzID=11784&aStudyGroupID=%22{GroupID}%22&aStartDate=%22{StartDate}%22&aEndDate=%22{EndDate}%22&aStudyTypeID=null";
            
            string request = await httpClient.GetStringAsync(sURL);

            var deserialization = JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, string>>>>(request);

            Content = deserialization["d"];
        }
    }
}

