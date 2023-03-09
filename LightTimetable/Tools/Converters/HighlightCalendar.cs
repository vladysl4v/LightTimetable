using System;
using System.Windows.Data;
using System.Globalization;
using System.Collections.Generic;


namespace LightTimetable.Tools.Converters
{
    public class HighlightCalendarConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return false;
            var date = (DateTime)values[0];
            var dates = values[1] as List<DateTime>;
            return dates.Contains(date);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
