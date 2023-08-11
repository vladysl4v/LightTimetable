using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Models;


namespace LightTimetable.ScheduleSources.Abstractions
{
    public interface IScheduleSource
    {
        public Task<Dictionary<DateTime, List<DataItem>>> LoadDataAsync(DateTime startDate, DateTime endDate);
    }
}
