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

        private readonly List<DataItem> _emptyList = new(0);
        public DateTime[] AvailableDates => _scheduleLoader.AvailableDates;

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

            await LoadServices(startDate, endDate);

            await _scheduleLoader.InitializeScheduleAsync(startDate, endDate,
                  new VnzOsvitaSource(_electricityService, _teamsService));
        }

        private async Task LoadServices(DateTime startDate, DateTime endDate)
        {
            if (Settings.Default.ShowOutages)
            {
                _electricityService = new ElectricityService(Settings.Default.OutagesCity, Settings.Default.OutagesGroup, Settings.Default.ShowPossibleOutages);
                await _electricityService.InitializeOutagesAsync();
            }
            else
            {
                _electricityService = null;
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
            else
            {
                _teamsService = null;
            }
        }
    }
}
