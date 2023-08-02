using System;
using LightTimetable.Models;


namespace LightTimetable.Services.Models
{
    public record SpecificEvent
    {
        public TimeInterval Time { get; init; }
        public string Name { get; init; }
        public string Link { get; init; }
        public DateOnly Date { get; init; }

        public SpecificEvent(TimeInterval time, string name, string link, DateOnly date)
        {
            Time = time;
            Name = name;
            Link = link;
            Date = date;
        }

        public override string ToString()
        {
            return Time + " - " + Name;
        }
    }
}