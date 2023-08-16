using System;
using System.Windows;
using System.Windows.Data;


namespace LightTimetable.Views.Converters
{
    public class AnythingToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter != null && (string)parameter == "Invert")
            {
                if (value == null || (string)value == string.Empty)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }

            if (value == null || value as string == string.Empty)
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
