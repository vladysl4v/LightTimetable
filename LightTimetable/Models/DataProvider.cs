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
        private readonly ScheduleLoader _scheduleLoader = new ScheduleLoader();
        private ElectricityService? _electricityService;
        private TeamsEventsService? _teamsService;
        private RiggedSchedule? _riggedSchedule;
        public DateTime[] AvailableDates => _scheduleLoader.AvailableDates;

        public List<DataItem> GetCurrentSchedule(DateTime date, out TimetableStatus status)
        {
            status = TimetableStatus.Default;

            if (_scheduleLoader.ScheduleData != null &&
               _scheduleLoader.ScheduleData.TryGetValue(date, out List<DataItem> correctDataItems)) 
            {
                return correctDataItems;
            }

            if (_riggedSchedule == null)
            {
                return new List<DataItem>(0);
            }

            var suitableSchedule = _riggedSchedule.GetRiggedSchedule(date);
            
            if (suitableSchedule != null && suitableSchedule.Any())
            {
                status = TimetableStatus.RiggedScheduleShown;
            }

            return suitableSchedule ?? new List<DataItem>(0);
        }

        public async Task<TimetableStatus> RefreshDataAsync()
        {
            await LoadScheduleAsync();

            if (_scheduleLoader.ScheduleData == null)
                return TimetableStatus.DataLoadingError;

            if (Settings.Default.ShowRiggedSchedule)
                _riggedSchedule = new RiggedSchedule(_scheduleLoader.ScheduleData);

            return TimetableStatus.Default;
        }

        private async Task LoadScheduleAsync()
        {
            var startOfTheWeek = DateTime.Today.AddDays(-(int)DateTime.Today.GetNormalDayOfWeek());

            var startDate = startOfTheWeek.AddDays(-14);
            var endDate = startOfTheWeek.AddDays(+13);

            await LoadServices(startDate, endDate);

            await _scheduleLoader.InitializeScheduleAsync(startDate, endDate,
                  new VnzOsvitaSource(_electricityService, _teamsService));
        }

        private async Task LoadServices(DateTime startDate, DateTime endDate)
        {
            _electricityService = null;
            _teamsService = null;

            if (Settings.Default.ShowOutages)
            {
                _electricityService = new ElectricityService(Settings.Default.OutagesCity, Settings.Default.OutagesGroup, Settings.Default.ShowPossibleOutages);
                
                await _electricityService.InitializeOutagesAsync();
            }

            if (Settings.Default.ShowTeamsEvents)
            {
                _teamsService = new TeamsEventsService();
                
                if (!Settings.Default.ShowOldEvents)
                {
                    startDate = (DateTime.Today < endDate) ? DateTime.Today : endDate;
                }
                
                await _teamsService.InitializeTeamsCalendarAsync(startDate, endDate);
            }
        }
    }
}
