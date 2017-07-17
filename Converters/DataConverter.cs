using System;
using Windows.UI.Xaml.Data;

namespace AvansApp.Converters
{
    public class DataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string data = string.Empty;
            
            if(value != null && !string.IsNullOrEmpty(value.ToString()) && !string.IsNullOrWhiteSpace(value.ToString()))
            {
                data = value.ToString();
            } else
            {
                data = "Niet bekend";
            }
            return data;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
