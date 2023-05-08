using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Models.Utilities;


namespace LightTimetable.Models.ScheduleSources
{
    public interface IScheduleSource
    {
        public Task<Dictionary<DateTime, List<DataItem>>?> LoadDataAsync(DateTime startDate, DateTime endDate);
    }
}
