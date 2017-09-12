using System;
using Windows.UI.Xaml.Data;

namespace AvansApp.Converters
{
    public class LesuurBackgroundColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return string.Empty;

            bool isBool = false;
            bool.TryParse(value.ToString(), out isBool);

            return (!isBool) ? "Green" : "Red";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
