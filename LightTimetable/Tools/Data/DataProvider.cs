using Newtonsoft.Json;

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Tools.Electricity;


namespace LightTimetable.Tools.Data
{
    public class DataProvider
    {
        private static string _rawData;
        private readonly ElectricityProvider _electricityProvider;

        public DataProvider()
        {
            _electricityProvider = new ElectricityProvider();
        }

        public Dictionary<DateTime, List<DataItem>> GetData()
        {
            if (_rawData == string.Empty)
            {
                return new Dictionary<DateTime, List<DataItem>>();
            }

            var incorrectData = DeserializeData(_rawData);
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
                    Data[Convert.ToDateTime(item["full_date"])].Add(new DataItem(item, _electricityProvider));
                }
                else
                {
                    Data[Convert.ToDateTime(item["full_date"])] = new List<DataItem>() { new DataItem(item, _electricityProvider) };
                }
            }
            return Data;
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
                _rawData = string.Empty;
            }

            _rawData = stringOutput;
        }
    }
}
