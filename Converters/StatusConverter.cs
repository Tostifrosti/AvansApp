using AvansApp.Helpers;
using System;
using Windows.UI.Xaml.Data;

namespace AvansApp.Converters
{
    public class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return "Mark_status_2".GetLocalized();

            string status = string.Empty;
            switch(value.ToString().ToUpper())
            {
                case "D":
                    status = "Mark_status_0".GetLocalized();
                    break;
                case "C":
                    status = "Mark_status_1".GetLocalized();
                    break;
                 default:
                    status = "Mark_status_2".GetLocalized();
                    break;
            }
            return status;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
