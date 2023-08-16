using System;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Models;
using LightTimetable.Properties;
using LightTimetable.Handlers.Utilities;
using LightTimetable.Handlers.Abstractions;
using LightTimetable.ScheduleSources.Exceptions;
using LightTimetable.ScheduleSources.Abstractions;



namespace LightTimetable.Handlers
{
    public class ScheduleHandler
    {
        public Dictionary<DateTime, List<DataItem>> ScheduleData { get; private set; }

        private readonly CacheManager<Dictionary<DateTime, List<DataItem>>?> _cache;

        private readonly IDateTimeHandler _dateTimeHandler;
        private readonly IScheduleSource? _scheduleSource;
        private readonly IUserSettings _settings;

        public ScheduleHandler(IUserSettings settings, IDateTimeHandler dateTimeHandler, IScheduleSource? scheduleSource = null)
        {
            _scheduleSource = scheduleSource;
            _dateTimeHandler = dateTimeHandler;
            _settings = settings;

            _cache = ConfigureCache();

            ScheduleData = new Dictionary<DateTime, List<DataItem>>();
        }

        public async Task InitializeScheduleAsync()
        {
            if (string.IsNullOrEmpty(_settings.StudyGroup) || _scheduleSource == null)
                return;

            DateTime startDate = _dateTimeHandler.AvailableDates.First();
            DateTime endDate = _dateTimeHandler.AvailableDates.Last();

            try
            {
                ScheduleData = await _scheduleSource.LoadDataAsync(startDate, endDate);

                _cache.SetCache(ScheduleData);
            }
            catch (ScheduleLoadingException)
            {
                var cached = LoadDataFromCache();
                if (cached != null)
                {
                    ScheduleData = cached;
                }
                throw;
            }

            if (ScheduleData.Any())
            {
                _dateTimeHandler.AvailableDates = ScheduleData.Keys.ToArray();
                RemoveObsoleteNotes(ScheduleData.First().Value.First().Id);
            }
        }

        private Dictionary<DateTime, List<DataItem>>? LoadDataFromCache()
        {
            var cacheData = _cache.GetCache();

            if (cacheData == null)
                return null;

            var cacheDate = (DateTime)_cache.GetCachedExtraData()["date"];
            var msgResult = MessageBox.Show("Виникла помилка пiд час завантаження розкладу\n" +
                                            $"Використати збережений розклад вiд {cacheDate.ToShortDateString()}?",
                                            "LightTimetable", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            return msgResult == MessageBoxResult.Yes ? cacheData : null;
        }

        private CacheManager<Dictionary<DateTime, List<DataItem>>?> ConfigureCache()
        {
            return new CacheManager<Dictionary<DateTime, List<DataItem>>?>("scheduleloader", TimeSpan.FromDays(3))
            {
                ExtraDataCondition = (data) => (string)data["studyGroup"] != _settings.StudyGroup,
                ExtraData = new Dictionary<string, object>
                {
                    { "studyGroup", _settings.StudyGroup },
                    { "date", DateTime.Today }
                }
            };
        }

        private void RemoveObsoleteNotes(uint oldestId)
        {
            foreach (var obsoleteId in _settings.Notes.Where(a => a.Key < oldestId))
            {
                _settings.Notes.Remove(obsoleteId.Key);
            }
            _settings.Save();
        }
    }
}
