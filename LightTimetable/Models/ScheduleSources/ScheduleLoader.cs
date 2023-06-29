using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Properties;
using LightTimetable.Models.Utilities;


namespace LightTimetable.Models.ScheduleSources
{
    public class ScheduleLoader
    {
        public Dictionary<DateTime, List<DataItem>>? ScheduleData { get; protected set; }
        public DateTime[] AvailableDates => ScheduleData?.Keys.ToArray() ?? Array.Empty<DateTime>();

        public async Task InitializeScheduleAsync(DateTime startDate, DateTime endDate, IScheduleSource scheduleSource)
        {
            if (string.IsNullOrEmpty(Settings.Default.StudyGroup))
                return;

            ScheduleData = await scheduleSource.LoadDataAsync(startDate, endDate);

            if (ScheduleData == null || !ScheduleData.Any())
                return;

            // Check for obsolete notes
            var oldestId = ScheduleData.First().Value.First().Id;

            foreach (var obsoleteId in Settings.Default.Notes.Where(a => a.Key < oldestId))
            {
                Settings.Default.Notes.Remove(obsoleteId.Key);
            }
            Settings.Default.Save();
        }
    }
}
