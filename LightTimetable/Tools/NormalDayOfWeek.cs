using System;
using System.Threading;


namespace LightTimetable.Tools
{
    /// <summary>
    /// DayOfWeek which counts days starting from Monday
    /// </summary>
    public enum NormalDayOfWeek
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }

    public static class DateTimeExtensions
    {
        public static NormalDayOfWeek GetNormalDayOfWeek(this DateTime date)
        {
            return date.DayOfWeek switch
            {
                DayOfWeek.Monday    => NormalDayOfWeek.Monday,
                DayOfWeek.Tuesday   => NormalDayOfWeek.Tuesday,
                DayOfWeek.Wednesday => NormalDayOfWeek.Wednesday,
                DayOfWeek.Thursday  => NormalDayOfWeek.Thursday,
                DayOfWeek.Friday    => NormalDayOfWeek.Friday,
                DayOfWeek.Saturday  => NormalDayOfWeek.Saturday,
                DayOfWeek.Sunday    => NormalDayOfWeek.Sunday,
                _ => throw new NotImplementedException()
            };
        }

    }
}
