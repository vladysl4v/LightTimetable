using System;


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
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday: return NormalDayOfWeek.Monday;
                case DayOfWeek.Tuesday: return NormalDayOfWeek.Tuesday;
                case DayOfWeek.Wednesday: return NormalDayOfWeek.Wednesday;
                case DayOfWeek.Thursday: return NormalDayOfWeek.Thursday;
                case DayOfWeek.Friday: return NormalDayOfWeek.Friday;
                case DayOfWeek.Saturday: return NormalDayOfWeek.Saturday;
                case DayOfWeek.Sunday: return NormalDayOfWeek.Sunday;
            }
            throw new NotImplementedException();
        }

    }
}
