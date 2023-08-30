using Microsoft.Extensions.DependencyInjection;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Models;
using LightTimetable.Services;
using LightTimetable.Properties;
using LightTimetable.ScheduleSources;
using LightTimetable.Handlers.Abstractions;
using LightTimetable.Services.Abstractions;
using LightTimetable.ScheduleSources.Exceptions;
using LightTimetable.ScheduleSources.Abstractions;


namespace LightTimetable.Handlers
{
    public class DataManager : IDataManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserSettings _settings;

        public event Action? OnStatusChanged;

        public DataManager(IUserSettings settings, SchedulesRepository schedulesRepository)
        {
            _settings = settings;

            IScheduleSource? source = schedulesRepository.GetScheduleSource();

            _serviceProvider = ConfigureServices(source);

            Dates = _serviceProvider.GetRequiredService<IDateTimeHandler>();
        }

        public IDateTimeHandler Dates { get; set; }

        private TimetableStatus _status;

        public TimetableStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnStatusChanged?.Invoke();
            }
        }

        public async Task LoadScheduleAsync()
        {
            Status = TimetableStatus.LoadingData;
            await InitializeServices();
            var scheduleLoader = _serviceProvider.GetRequiredService<ScheduleHandler>();

            try
            {
                await scheduleLoader.InitializeScheduleAsync();
                Status = TimetableStatus.Default;
            }
            catch (ScheduleLoadingException)
            {
                Status = TimetableStatus.DataLoadingError;
            }

            if (_settings.ShowRiggedSchedule)
            {
                var predictor = _serviceProvider.GetService<IPredictionHandler>();
                predictor?.SetBaseSchedule(scheduleLoader.ScheduleData);
            }
        }

        public List<DataItem> GetSchedule(DateTime date)
        {
            Status = TimetableStatus.Default;

            var scheduleLoader = _serviceProvider.GetRequiredService<ScheduleHandler>();

            if (scheduleLoader.ScheduleData.TryGetValue(date, out List<DataItem> correctDataItems))
            {
                return correctDataItems;
            }

            var predictor = _serviceProvider.GetService<IPredictionHandler>();

            if (predictor == null)
            {
                return new List<DataItem>(0);
            }

            var suitableSchedule = predictor.GetRiggedSchedule(date);

            if (suitableSchedule.Any())
            {
                Status = TimetableStatus.RiggedScheduleShown;
            }

            return suitableSchedule;
        }

        private IServiceProvider ConfigureServices(IScheduleSource? source)
        {
            var services = new ServiceCollection();

            services.AddSingleton<IUserSettings>(_settings);
            services.AddHttpClient();
            
            services.AddSingleton<IDateTimeHandler, DateTimeHandler>();

            if (source != null)
            {
                services.AddSingleton(typeof(IScheduleSource), source.GetType());
            }

            

            if (_settings.ShowTeamsEvents)
            {
                services.AddSingleton<IEventsService, TeamsEventsService>();
            }
            if (_settings.ShowOutages)
            {
                services.AddSingleton<IElectricityService, DtekElectricityService>();
            }

            services.AddTransient<DataItemBuilder>();

            services.AddSingleton<ScheduleHandler>();

            if (_settings.ShowRiggedSchedule)
            {
                services.AddSingleton<IPredictionHandler, PredictionHandler>();
            }

            return services.BuildServiceProvider();
        }

        private async Task InitializeServices()
        {
            if (_settings.ShowOutages)
            {
                var electricityService = _serviceProvider.GetRequiredService<IElectricityService>();
                await electricityService.InitializeAsync();
            }

            if (_settings.ShowTeamsEvents)
            {
                var eventService = _serviceProvider.GetRequiredService<IEventsService>();
                await eventService.InitializeAsync();
            }
        }

    }
}
