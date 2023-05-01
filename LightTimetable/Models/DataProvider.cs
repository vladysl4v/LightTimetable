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
        private RiggedSchedule? _riggedSchedule;
        private BaseScheduleSource? _scheduleSource;

        public DateTime[] AvailableDates => _scheduleSource?.AvailableDates ?? Array.Empty<DateTime>();

        public List<DataItem> GetCurrentSchedule(DateTime date, out bool isRigged)
        {
            isRigged = false;

            if (_scheduleSource!.ScheduleDictionary != null && _scheduleSource!.ScheduleDictionary.TryGetValue(date, out List<DataItem> correctDataItems)) 
                return correctDataItems;

            if (_riggedSchedule == null)
                return new List<DataItem>();

            var suitableSchedule = _riggedSchedule.GetRiggedSchedule(date);
            isRigged = suitableSchedule.Any();

            return suitableSchedule;

        }

        public void UpdateRenames(DisciplinePair renamePair)
        {
            _scheduleSource!.UpdateRenames(renamePair);
        }

        public async Task<bool> RefreshDataAsync()
        {
            await LoadScheduleAsync();

            if (_scheduleSource!.ScheduleDictionary == null)
                return false;

            if (Settings.Default.ShowRiggedSchedule)
                _riggedSchedule = new RiggedSchedule(_scheduleSource.ScheduleDictionary);

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

            _scheduleSource ??= new VnzOsvitaSource();

            await _scheduleSource.InitializeScheduleAsync(startDate, endDate);
        }

    }
}
