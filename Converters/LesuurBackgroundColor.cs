using System;
using Windows.UI.Xaml.Data;

namespace AvansApp.Converters
{
    public class LesuurBackgroundColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string returnvalue;
            bool isBool = false;
            bool.TryParse(value.ToString(), out isBool);
            if (!isBool)
            {
                returnvalue = "Green";
            }
            else
            {
                returnvalue = "Red";
            }
            return returnvalue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
