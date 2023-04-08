using System;


namespace LightTimetable.Tools
{

    public static class NormalDayOfWeekExtensions
    {
        public static NormalDayOfWeek GetNormalDayOfWeek(this DateTime date) => date.DayOfWeek.Normalize();

        public static NormalDayOfWeek Normalize(this DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
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
