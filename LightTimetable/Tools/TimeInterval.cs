using System;


namespace LightTimetable.Tools
{
    public struct TimeInterval
    {
        public TimeOnly Start { get; set; }
        public TimeOnly End { get; set; }

        public TimeInterval(TimeOnly intervalBegin, TimeOnly intervalEnd)
        {
            Start = intervalBegin;
            End = intervalEnd;
        }

        public override string ToString()
        {
            var startString = Start.ToString("HH:mm");
            var endString = End.ToString("HH:mm");
            return $"{startString}-{endString}";
        }
    }
}
