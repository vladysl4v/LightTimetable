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
        private static string _serializedData;

        private Dictionary<DateTime, List<DataItem>> _scheduleTable;
        private RiggedScheduleDictionary _riggedSchedule;
        private ElectricityProvider _electricity;
        private DateTime[] _availableDates;

        public DataProvider()
        {
            ElectricityProvider.BlackoutsInitialized += ElectricityReady;
        }

        public static event Action DataInitialized;

        #region Properties

        public static bool IsDataInitialized { get; set; }

        public Dictionary<DateTime, List<DataItem>> ScheduleDictionary
        {
            get
            {
                _scheduleTable ??= GetData();
                _riggedSchedule ??= InitializeRiggedSchedule();
                return _scheduleTable;
            } 
            set => _scheduleTable = value;
        }

        public DateTime[] AvailableDates
        {
            get => _availableDates ??= ScheduleDictionary.Keys.ToArray();
            set => _availableDates = value;
        }

        #endregion

        #region Methods

        private static void OnDataInitialized()
        {
            if (IsDataInitialized && ElectricityProvider.IsBlackoutsInitialized)
            {
                DataInitialized.Invoke();
            }
        }

        public static async Task InitializeDataAsync()
        {
            string startDate = DateTime.Now.AddDays(-14).ToShortDateString();
            string endDate = DateTime.Now.AddDays(+14).ToShortDateString();
            using HttpClient httpClient = new HttpClient();
            string sURL = $"https://vnz.osvita.net/BetaSchedule.asmx/GetScheduleDataX?&aVuzID=11784&aStudyGroupID=%22{Properties.Settings.Default.StudyGroup}%22&aStartDate=%22{startDate}%22&aEndDate=%22{endDate}%22&aStudyTypeID=null";
            string stringOutput = await httpClient.GetStringAsync(sURL);

            _serializedData = stringOutput.Length < 15 ? string.Empty : stringOutput;
            IsDataInitialized = true;
            OnDataInitialized();
        }

        public void ReloadData()
        {
            ScheduleDictionary = GetData();
            AvailableDates = ScheduleDictionary.Keys.ToArray();
            _electricity.ReloadData();
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

        private RiggedScheduleDictionary InitializeRiggedSchedule()
        {
            var isWeekPrimary = true;
            var previousDayOfWeek = NormalDayOfWeek.Sunday;

            var primaryWeek = new Dictionary<int, List<DataItem>>();
            var secondaryWeek = new Dictionary<int, List<DataItem>>();

            foreach (var dayItems in _scheduleTable.Reverse())
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

            var lastWeekPrimariness = IsWeekPrimary(_scheduleTable.Last().Key);

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

        private Dictionary<DateTime, List<DataItem>> GetData()
        {
            if (_serializedData is "" or null)
            {
                return new Dictionary<DateTime, List<DataItem>>();
            }

            var incorrectData = DeserializeData(_serializedData);
            return StructureData(incorrectData);
        }

        private List<Dictionary<string, string>> DeserializeData(string rawString)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, string>>>>(rawString)["d"];
        }

        private Dictionary<DateTime, List<DataItem>> StructureData(List<Dictionary<string, string>>? rawDataItems)
        {
            var Data = new Dictionary<DateTime, List<DataItem>>();

            foreach (var item in rawDataItems)
            {
                if (Data.ContainsKey(Convert.ToDateTime(item["full_date"])))
                {
                    Data[Convert.ToDateTime(item["full_date"])].Add(new DataItem(item, _electricity.FindIntersections(item)));
                }
                else
                {
                    Data[Convert.ToDateTime(item["full_date"])] = new List<DataItem>() { new DataItem(item, _electricity.FindIntersections(item)) };
                }
            }

            return Data;
        }

        private void ElectricityReady()
        {
            _electricity = new ElectricityProvider();
            OnDataInitialized();
        }

        #endregion
    }
}
