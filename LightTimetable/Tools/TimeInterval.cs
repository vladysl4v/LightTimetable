using System;


namespace LightTimetable.Tools
{
    public struct TimeInterval
    {
        public TimeOnly Start { get; set; }
        public TimeOnly End { get; set; }

        private readonly string _stringTime;

        public TimeInterval(string studyBegin, string studyEnd,  int timeSpan = 0)
        {
            Start = TimeOnly.Parse(studyBegin).AddHours(timeSpan);
            End = TimeOnly.Parse(studyEnd).AddHours(timeSpan);
            _stringTime = GenerateString();
        }

        private string GenerateString()
        {
            var startString = Start.ToShortTimeString();
            var endString = End.ToShortTimeString();
            if (Start.Hour < 10) startString = "0" + startString;
            if (End.Hour < 10) endString = "0" + endString;

            return $"{startString}-{endString}";
        }

        public override string ToString() => _stringTime;
    }
}
