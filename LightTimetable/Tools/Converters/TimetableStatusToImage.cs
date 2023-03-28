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

            var param = (string)parameter; 

            if (thisValue == TimetableStatus.Loading)
            {
                switch (param)
                {
                    case "Icon":    return "../Assets/Loading.png";
                    case "ToolTip": return "Дані розкладу завантажуються";
                }
            }
            if (thisValue == TimetableStatus.Warning)
            {
                switch (param)
                {
                    case "Icon":    return "../Assets/Warning.png";
                    case "ToolTip": return "Відображається не справжній розклад, а згенерований на основі минулих тижнів";
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
