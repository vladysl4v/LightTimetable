using Microsoft.Extensions.DependencyInjection;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Tools;
using LightTimetable.Common;
using LightTimetable.Properties;
using LightTimetable.Models.Services;
using LightTimetable.Models.Utilities;



namespace LightTimetable.Models
{
    public class DataProvider
    {
        private readonly ScheduleLoader _scheduleLoader = new ScheduleLoader();
        private IServiceProvider _serviceProvider; 
        private SchedulePredictor? _riggedSchedule;

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
            
            if (suitableSchedule.Any())
            {
                status = TimetableStatus.RiggedScheduleShown;
            }

            return suitableSchedule;
        }

        public async Task<TimetableStatus> RefreshDataAsync()
        {
            await LoadScheduleAsync();

            if (_scheduleLoader.ScheduleData == null)
                return TimetableStatus.DataLoadingError;

            if (Settings.Default.ShowRiggedSchedule)
                _riggedSchedule = new SchedulePredictor(_scheduleLoader.ScheduleData);

            return TimetableStatus.Default;
        }

        private async Task LoadScheduleAsync()
        {
            var startOfTheWeek = DateTime.Today.AddDays(-(int)DateTime.Today.GetNormalDayOfWeek());

            var startDate = startOfTheWeek.AddDays(-14);
            var endDate = startOfTheWeek.AddDays(+13);

            // FAKE DATE
            startDate = new DateTime(2023, 01, 20);
            endDate = new DateTime(2023, 02, 10);
            //

            _serviceProvider = ConfigureServices();
            await InitializeServices(startDate, endDate);

            var scheduleSource = ScheduleActivator.GetScheduleSource(
                    Settings.Default.ScheduleSource, _serviceProvider);
            
            if (scheduleSource != null)
            {
                await _scheduleLoader.InitializeScheduleAsync(startDate, endDate, scheduleSource);
            }
        }
        
        private IServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();
            
            serviceCollection.AddHttpClient();

            if (Settings.Default.ShowTeamsEvents)
            {
                serviceCollection.AddSingleton<IEventsService, TeamsEventsService>();
            }

            if (Settings.Default.ShowOutages)
            {
                serviceCollection.AddSingleton<IElectricityService, YasnoElectricityService>((_) =>
                {
                    return new YasnoElectricityService(Settings.Default.OutagesCity,
                        Settings.Default.OutagesGroup, Settings.Default.ShowPossibleOutages, _serviceProvider);
                });
            }

            return serviceCollection.BuildServiceProvider();
        }

        private async Task InitializeServices(DateTime startDate, DateTime endDate)
        {
            var electricityService = _serviceProvider.GetService<IElectricityService>();
            if (electricityService != null)
            {
                await electricityService.InitializeAsync();
            }

            var eventService = _serviceProvider.GetService<IEventsService>();
            if (eventService != null)
            {
                DateTime eventStartDate = startDate;
                if (!Settings.Default.ShowOldEvents)
                {
                    eventStartDate = (DateTime.Today < endDate) ? DateTime.Today : endDate;
                }
                await eventService.InitializeAsync(eventStartDate, endDate);
            }
        }
    }
}
