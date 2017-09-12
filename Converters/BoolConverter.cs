using AvansApp.Helpers;
using System;
using Windows.UI.Xaml.Data;

namespace AvansApp.Converters
{
    public class BoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string result = string.Empty;
            if (value == null)
                return result;

            bool isBool = false;
            bool.TryParse(value.ToString(), out isBool);
            if(!isBool) {
                result = "Classroom_status_0".GetLocalized();
            } else {
                result = "Classroom_status_1".GetLocalized();
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
