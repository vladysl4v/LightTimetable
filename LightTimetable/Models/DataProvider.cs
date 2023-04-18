using Newtonsoft.Json;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Tools;
using LightTimetable.Models.Utilities;


namespace LightTimetable.Models
{
    public class DataProvider
    {
        private Dictionary<DateTime, List<DataItem>> _scheduleDictionary;
        private RiggedSchedule _riggedSchedule;
        private bool _isDataSuccessfullyLoaded;

        public DataProvider()
        {
            AvailableDates = Array.Empty<DateTime>();
        }

        public DateTime[] AvailableDates { get; private set; }

        public List<DataItem> GetCurrentSchedule(DateTime date, out bool isRigged)
        {
            isRigged = false;
            if (!_scheduleDictionary.TryGetValue(date, out List<DataItem> correctDataItems))
            {
                if (!Properties.Settings.Default.ShowRiggedSchedule)
                    return new List<DataItem>();

                var suitableSchedule = _riggedSchedule.GetRiggedSchedule(date);
                isRigged = suitableSchedule.Any();
                return suitableSchedule;
            }

            return correctDataItems;
        }

        public void UpdateRenames(DisciplinePair renamePair)
        {
            foreach (var item in from dateItems in _scheduleDictionary.Values
                     from item in dateItems
                     where item.Discipline.Original == renamePair.Original
                     select item)
            {
                item.Discipline.Modified = renamePair.Modified;
            }
        }

        public async Task<bool> RefreshDataAsync()
        {
            _scheduleDictionary = await LoadScheduleAsync();
            _riggedSchedule = new RiggedSchedule(_scheduleDictionary);
            AvailableDates = _scheduleDictionary.Any() ? _scheduleDictionary.Keys.ToArray() : Array.Empty<DateTime>();
            return _isDataSuccessfullyLoaded;
        }

        #region Data initialization

        private async Task<Dictionary<DateTime, List<DataItem>>> LoadScheduleAsync()
        {
            await ElectricityPlugin.InitializeBlackoutsAsync();

            var startOfTheWeek = DateTime.Today.AddDays(-(int)DateTime.Today.GetNormalDayOfWeek());

            var startDate = startOfTheWeek.AddDays(-14).ToShortDateString();
            var endDate = startOfTheWeek.AddDays(+13).ToShortDateString();

            var url = $"https://vnz.osvita.net/BetaSchedule.asmx/GetScheduleDataX?&aVuzID=11784&aStudyGroupID=%22{Properties.Settings.Default.StudyGroup}%22&aStartDate=%22{startDate}%22&aEndDate=%22{endDate}%22&aStudyTypeID=null";

            var request = await HttpRequestService.LoadStringAsync(url);

            _isDataSuccessfullyLoaded = request.IsSuccessful;

            return ConvertToCollection(request.Response);
        }


        private Dictionary<DateTime, List<DataItem>> ConvertToCollection(string serializedData)
        {
            if (serializedData == "" || serializedData.Length < 15)
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

            CheckForObsoleteNotes(result);

            return result;
        }

        private void CheckForObsoleteNotes(Dictionary<DateTime, List<DataItem>> schedule)
        {
            if (schedule.Any())
                return;

            var oldestId = schedule.First().Value.First().Id;

            foreach (var obsoleteId in Properties.Settings.Default.Notes.Where(a => a.Key < oldestId))
            {
                Properties.Settings.Default.Notes.Remove(obsoleteId.Key);
            }
            Properties.Settings.Default.Save();
        }

        #endregion
    }
}
