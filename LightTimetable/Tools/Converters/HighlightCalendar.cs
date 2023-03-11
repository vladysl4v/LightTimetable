using System;
using System.Windows.Data;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;


namespace LightTimetable.Tools.Converters
{
    public class HighlightCalendarConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values[0] == null)
                return false;
            var currentDate = (DateTime)values[0];
            var availableDates = values[1] as DateTime[];
            return availableDates.Contains(currentDate);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
