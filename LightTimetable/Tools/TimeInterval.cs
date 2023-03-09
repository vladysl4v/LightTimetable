using System;
using System.Text.RegularExpressions;


namespace LightTimetable.Tools
{
    public class TimeInterval
    {
        public TimeOnly Start { get; set; }
        public TimeOnly End { get; set; }

        private readonly Regex _hyphenSplitter = new("-");

        public TimeInterval(string stringInterval, int timeSpan = 0)
        {
            string[] timeInterval = _hyphenSplitter.Split(stringInterval);
            Start = TimeOnly.Parse(timeInterval[0]).AddHours(timeSpan);
            End = TimeOnly.Parse(timeInterval[1]).AddHours(timeSpan);
        }


        public override string ToString()
        {
            string startString = Start.ToShortTimeString();
            string endString = End.ToShortTimeString();
            if (Start.Hour < 10) startString = "0" + startString;
            if (End.Hour < 10) endString = "0" + endString;

            return $"{startString}-{endString}";
        }
    }
}
