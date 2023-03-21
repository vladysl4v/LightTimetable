using System;
using System.Windows.Data;

namespace LightTimetable.Tools.Converters
{
    public class TimetableStatusToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is not TimetableStatus thisValue)
                return false;

            if (thisValue == TimetableStatus.Loading)
            {
                return "../Assets/Loading.gif";
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
