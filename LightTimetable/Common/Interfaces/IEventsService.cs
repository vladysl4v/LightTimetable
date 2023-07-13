using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace LightTimetable.Common
{
    public interface IEventsService
    {
        public List<SpecificEvent> GetMeetingsInformation(DateTime date, TimeInterval time);
        public Task InitializeAsync(DateTime start, DateTime end);
    }
}