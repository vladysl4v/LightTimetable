using Newtonsoft.Json;

using System;
using System.Linq;
using System.Net.Http;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Tools;
using LightTimetable.Models.Electricity;

using RiggedScheduleDictionary =
    System.Collections.Generic.Dictionary<bool, System.Collections.Generic.Dictionary<int,
        System.Collections.Generic.List<LightTimetable.Models.DataItem>>>;


namespace LightTimetable.Models
{
    public class DataProvider
    {
        private RiggedScheduleDictionary _riggedSchedule;

        public DataProvider()
        {
            AvailableDates = Array.Empty<DateTime>();
        }

        public Dictionary<DateTime, List<DataItem>> ScheduleDictionary { get; set; }
        public DateTime[] AvailableDates { get; set; }

        public async Task RefreshDataAsync()
        {
            ScheduleDictionary = await LoadScheduleAsync();
            _riggedSchedule = InitializeRiggedSchedule();
            AvailableDates = ScheduleDictionary.Keys.ToArray();
        }

        public List<DataItem> GetRiggedSchedule(DateTime date, out bool showWarning)
        {
            showWarning = false;

            if (_riggedSchedule == null)
                return new List<DataItem>();

            if (!_riggedSchedule[IsWeekPrimary(date)].TryGetValue((int)date.GetNormalDayOfWeek(), out List<DataItem> suitableList))
                return new List<DataItem>();

            var result = new List<DataItem>();
            suitableList.ForEach(x => result.Add(new DataItem(x, date)));

            showWarning = true;

            return result;
        }

        private async Task<Dictionary<DateTime, List<DataItem>>> LoadScheduleAsync()
        {
            await ElectricityPlugin.InitializeBlackoutsAsync();

            var startOfTheWeek = DateTime.Today.AddDays(-(int)DateTime.Today.GetNormalDayOfWeek());

            var startDate = startOfTheWeek.AddDays(-14).ToShortDateString();
            var endDate = startOfTheWeek.AddDays(+13).ToShortDateString();

            using var httpClient = new HttpClient();

            var serializedData = "";

            for (var retries = 0; retries < 5; retries++)
            {
                var sURL = $"https://vnz.osvita.net/BetaSchedule.asmx/GetScheduleDataX?&aVuzID=11784&aStudyGroupID=%22{Properties.Settings.Default.StudyGroup}%22&aStartDate=%22{startDate}%22&aEndDate=%22{endDate}%22&aStudyTypeID=null";
                try
                {
                    serializedData = await httpClient.GetStringAsync(sURL);
                }
                catch
                {
                    break;
                }

                if (serializedData.Length < 15)
                {
                    serializedData = "";
                }
            }

            return ConvertToCollection(serializedData);
        }

        private RiggedScheduleDictionary InitializeRiggedSchedule()
        {
            var isWeekPrimary = true;
            var previousDayOfWeek = NormalDayOfWeek.Sunday;

            var primaryWeek = new Dictionary<int, List<DataItem>>();
            var secondaryWeek = new Dictionary<int, List<DataItem>>();

            foreach (var dayItems in ScheduleDictionary.Reverse())
            {
                if (dayItems.Key.GetNormalDayOfWeek() > previousDayOfWeek)
                {
                    if (!isWeekPrimary)
                        break;
                    isWeekPrimary = false;
                }

                if (isWeekPrimary)
                    primaryWeek.Add((int)dayItems.Key.GetNormalDayOfWeek(), dayItems.Value);
                else
                    secondaryWeek.Add((int)dayItems.Key.GetNormalDayOfWeek(), dayItems.Value);

                previousDayOfWeek = dayItems.Key.GetNormalDayOfWeek();
            }

            var lastWeekPrimariness = IsWeekPrimary(ScheduleDictionary.Last().Key);

            return new RiggedScheduleDictionary
            {
                { !lastWeekPrimariness, secondaryWeek },
                { lastWeekPrimariness, primaryWeek }
            };
        }

        private bool IsWeekPrimary(DateTime date)
        {
            var intPrimariness = new GregorianCalendar().GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday) % 2;
            return intPrimariness == 1;
        }


        private Dictionary<DateTime, List<DataItem>> ConvertToCollection(string serializedData)
        {
            if (serializedData == "")
            {
                return new Dictionary<DateTime, List<DataItem>>();
            }

            var rawDataItems = JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, string>>>>(serializedData)["d"].GroupBy(x => x["full_date"]);

            var result = new Dictionary<DateTime, List<DataItem>>();

            foreach (var group in rawDataItems)
            {
                result.Add(Convert.ToDateTime(group.Key), new List<DataItem>());

                foreach (var lesson in group)
                {
                    result[Convert.ToDateTime(group.Key)].Add(new DataItem(lesson));
                }
            }

            return result;
        }

    }
}
