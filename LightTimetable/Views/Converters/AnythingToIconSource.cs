using System;
using System.Windows.Data;


namespace LightTimetable.Views.Converters
{
    public class AnythingToIconSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter == null || string.IsNullOrEmpty((string)value))
                return false;

            return (string)parameter switch
            {
                "Note" => "/Assets/DataGrid/Notes.png",
                "Subgroup" => "/Assets/DataGrid/Subgroup.png",
                _ => string.Empty
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}