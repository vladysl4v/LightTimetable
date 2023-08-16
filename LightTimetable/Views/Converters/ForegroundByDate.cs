using System;
using System.Windows.Data;
using System.Windows.Media;


namespace LightTimetable.Views.Converters
{
    public class ForegroundByDateConverter : IValueConverter
    {
        private readonly SolidColorBrush _defaultFontColor = new(Colors.Black);
        private readonly SolidColorBrush _highlightedFontColor = new(Colors.Green);

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DateTime currentDate)
            {
                return currentDate.Date == DateTime.Now.Date ? _highlightedFontColor : _defaultFontColor;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
