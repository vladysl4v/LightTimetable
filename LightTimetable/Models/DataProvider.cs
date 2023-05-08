using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Tools;
using LightTimetable.Properties;
using LightTimetable.Models.Services;
using LightTimetable.Models.Utilities;
using LightTimetable.Models.ScheduleSources;


namespace LightTimetable.Models
{
    public class DataProvider
    {
        private readonly ScheduleLoader _scheduleLoader = new();
        private RiggedSchedule? _riggedSchedule;

        private readonly List<DataItem> _emptyList = new(0);

        public DateTime[] AvailableDates { get; private set; } = Array.Empty<DateTime>();

        public List<DataItem> GetCurrentSchedule(DateTime date, out bool isRigged)
        {
            isRigged = false;

            if (_scheduleLoader.ScheduleDictionary != null && _scheduleLoader.ScheduleDictionary.TryGetValue(date, out List<DataItem> correctDataItems)) 
                return correctDataItems;

            if (_riggedSchedule == null)
                return _emptyList;

            var suitableSchedule = _riggedSchedule.GetRiggedSchedule(date);
            isRigged = suitableSchedule.Any();

            return suitableSchedule;

        }

        public void UpdateRenames(DisciplinePair renamePair)
        {
            _scheduleLoader.UpdateRenames(renamePair);
        }

        public async Task<bool> RefreshDataAsync()
        {
            await LoadScheduleAsync();

            if (_scheduleLoader.ScheduleDictionary == null)
                return false;

            if (Settings.Default.ShowRiggedSchedule)
                _riggedSchedule = new RiggedSchedule(_scheduleLoader.ScheduleDictionary);

            return true;
        }

        private async Task LoadScheduleAsync()
        {
            var startOfTheWeek = DateTime.Today.AddDays(-(int)DateTime.Today.GetNormalDayOfWeek());

            var startDate = startOfTheWeek.AddDays(-14);
            var endDate = startOfTheWeek.AddDays(+13);

            if (Settings.Default.ShowBlackouts)
            {
                await ElectricityPlugin.InitializeBlackoutsAsync();
            }

            if (Settings.Default.ShowTeamsEvents)
            {
                var teamsStartDate = Settings.Default.ShowOldEvents ? startDate : DateTime.Today;
                await TeamsEventsPlugin.InitializeTeamsCalendarAsync(teamsStartDate, endDate);
            }

            await _scheduleLoader.InitializeScheduleAsync(startDate, endDate, new VnzOsvitaSource());

            AvailableDates = _scheduleLoader.ScheduleDictionary?.Keys.ToArray() ?? Array.Empty<DateTime>();
        }

    }
}
