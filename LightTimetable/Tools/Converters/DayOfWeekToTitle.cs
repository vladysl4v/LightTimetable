using System;
using System.Windows.Data;


namespace LightTimetable.Tools.Converters
{
    public class DayOfWeekToTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string localized = ((DateTime)value).DayOfWeek switch
            {
                DayOfWeek.Monday => "понеділок",
                DayOfWeek.Tuesday => "вівторок",
                DayOfWeek.Wednesday => "середу",
                DayOfWeek.Thursday => "четвер",
                DayOfWeek.Friday => "п'ятницю",
                DayOfWeek.Saturday => "суботу",
                DayOfWeek.Sunday => "неділю",
                _ => string.Empty
            };
            return "Розклад на " + localized;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
