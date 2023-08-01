using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Models;


namespace LightTimetable.Handlers.Abstractions
{
    public interface IDataManager
    {
        public event Action? OnStatusChanged;
        public TimetableStatus Status { get; set; }
        public IDateTimeHandler Dates { get; protected set; }
        public Task LoadScheduleAsync();
        public List<DataItem> GetSchedule(DateTime date);
    }
}
