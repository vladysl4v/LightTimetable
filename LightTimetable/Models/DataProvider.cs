using Newtonsoft.Json;

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Models.Electricity;


namespace LightTimetable.Models
{
    public class DataProvider
    {
        private readonly ElectricityProvider _electricity;
        private static string _serializedData;

        public DataProvider()
        {
            _electricity = new ElectricityProvider();
        }

        #region Properties

        private Dictionary<DateTime, List<DataItem>> _scheduleTable;
        private DateTime[] _availableDates;

        public Dictionary<DateTime, List<DataItem>> ScheduleDictionary
        {
            get => _scheduleTable ??= GetData();
            set => _scheduleTable = value;
        }

        public DateTime[] AvailableDates
        {
            get => _availableDates ??= ScheduleDictionary.Keys.ToArray();
            set => _availableDates = value;
        }

        #endregion

        #region Methods
        public void ReloadData()
        {
            ScheduleDictionary = GetData();
            AvailableDates = ScheduleDictionary.Keys.ToArray();
            _electricity.ReloadData();
        }

        public static async Task InitializeDataAsync()
        {
            string startDate = DateTime.Now.AddDays(-14).ToShortDateString();
            string endDate = DateTime.Now.AddDays(+14).ToShortDateString();
            using HttpClient httpClient = new HttpClient();
            string sURL = $"https://vnz.osvita.net/BetaSchedule.asmx/GetScheduleDataX?&aVuzID=11784&aStudyGroupID=%22{Properties.Settings.Default.StudyGroup}%22&aStartDate=%22{startDate}%22&aEndDate=%22{endDate}%22&aStudyTypeID=null";
            string stringOutput = await httpClient.GetStringAsync(sURL);

            if (stringOutput.Length < 15)
            {
                _serializedData = string.Empty;
            }

            _serializedData = stringOutput;
        }

        private Dictionary<DateTime, List<DataItem>> GetData()
        {
            if (_serializedData == string.Empty)
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

        #endregion
    }
}
