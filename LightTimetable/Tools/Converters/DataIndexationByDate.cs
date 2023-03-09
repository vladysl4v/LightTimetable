using System;
using System.Windows.Data;
using System.Collections.Generic;

using LightTimetable.Tools.Data;


namespace LightTimetable.Tools.Converters
{
    public class DataIndexationByDateConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[1] == null || (DateTime)values[1] == DateTime.MinValue)
                return new List<object>();

            var providedData = (Dictionary<DateTime, List<DataItem>>)values[0];
            
            return providedData.TryGetValue((DateTime)values[1], out List<DataItem> correctDataItems) ? correctDataItems : new List<DataItem>();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
