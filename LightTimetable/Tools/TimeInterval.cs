using System;


namespace LightTimetable.Tools
{
    public readonly struct TimeInterval
    {
        public TimeOnly Start { get; }
        public TimeOnly End { get; }

        public TimeInterval(TimeOnly start, TimeOnly end)
        {
            Start = start;
            End = end;
        }

        public override string ToString()
        {
            var startString = Start.ToString("HH:mm");
            var endString = End.ToString("HH:mm");
            return $"{startString}-{endString}";
        }
    }
}
