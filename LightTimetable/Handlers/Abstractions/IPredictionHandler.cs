using System;
using System.Collections.Generic;

using LightTimetable.Models;


namespace LightTimetable.Handlers.Abstractions
{
    public interface IPredictionHandler
    {
        public List<DataItem> GetRiggedSchedule(DateTime date);
        public void SetBaseSchedule(Dictionary<DateTime, List<DataItem>> fullSchedule);
    }
}
