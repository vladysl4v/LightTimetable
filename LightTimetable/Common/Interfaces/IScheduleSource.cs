using System;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace LightTimetable.Common
{
    public interface IScheduleSource
    {
        public Task<Dictionary<DateTime, List<DataItem>>?> LoadDataAsync(DateTime startDate, DateTime endDate);
    }
}
