using System;

using LightTimetable.Tools;


namespace LightTimetable.Common
{
    public record SpecificEvent
    {
        public TimeInterval Time { get; init; }
        public string Name { get; init; }
        public Uri Link { get; init; }
        public DateOnly Date { get; init; }

        public SpecificEvent(TimeInterval time, string name, Uri link, DateOnly date)
        {
            Time = time;
            Name = name;
            Link = link;
            Date = date;
        }

        public override string ToString()
        {
            return Time.ToString() + " - " + Name;
        }
    }
}