using System;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Tools;
using LightTimetable.Properties;
using LightTimetable.Models.Utilities;


namespace LightTimetable.Models.ScheduleSources
{
    public class ScheduleLoader
    {
        public Dictionary<DateTime, List<DataItem>>? ScheduleData { get; protected set; }
        public DateTime[] AvailableDates => ScheduleData?.Keys.ToArray() ?? Array.Empty<DateTime>();
        private CacheManager<Dictionary<DateTime, List<DataItem>>?> _cache;

        public CacheManager<Dictionary<DateTime, List<DataItem>>?> GetCacheManager()
        {
            return new CacheManager<Dictionary<DateTime, List<DataItem>>?>("scheduleloader", TimeSpan.FromDays(3))
            {
                ExtraDataCondition = (data) => ((string)data["studyGroup"] != Settings.Default.StudyGroup),
                ExtraData = new Dictionary<string, object>
                {
                    { "studyGroup", Settings.Default.StudyGroup },
                    { "date", DateTime.Today }
                }
            };    
        }

        public async Task InitializeScheduleAsync(DateTime startDate, DateTime endDate, IScheduleSource scheduleSource)
        {
            if (string.IsNullOrEmpty(Settings.Default.StudyGroup))
                return;

            _cache = GetCacheManager();

            ScheduleData = await scheduleSource.LoadDataAsync(startDate, endDate);
            
            _cache.SetCache(ScheduleData);
            
            if (ScheduleData == null)
            {
                ScheduleData = LoadDataFromCache();
                return;
            }
            
            if (ScheduleData.Any())
            {
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
                                            $"Використати кешований розклад вiд {cacheDate.ToShortDateString()}?",
                                            "LightTimetable", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (msgResult == MessageBoxResult.Yes)
            {
                return cacheData;
            }

            return null;
        }

        private void RemoveObsoleteNotes(uint oldestId)
        {
            foreach (var obsoleteId in Settings.Default.Notes.Where(a => a.Key < oldestId))
            {
                Settings.Default.Notes.Remove(obsoleteId.Key);
            }
            Settings.Default.Save();
        }
    }
}
