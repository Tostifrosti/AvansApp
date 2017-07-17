using System;
using Windows.UI.Xaml.Data;

namespace AvansApp.Converters
{
    public class BoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string returnvalue;
            bool isBool = false;
            bool.TryParse(value.ToString(), out isBool);
            if(!isBool) {
                returnvalue = "Vrij";
            } else {
                returnvalue = "Bezet";
            }
            return returnvalue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
