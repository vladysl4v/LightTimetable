using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Models;
using LightTimetable.Services.Models;


namespace LightTimetable.Services.Abstractions
{
    public interface IEventsService
    {
        public List<SpecificEvent> GetMeetingsInformation(DateTime date, TimeInterval time);
        public Task InitializeAsync();
    }
}