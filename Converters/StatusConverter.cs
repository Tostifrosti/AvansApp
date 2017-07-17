using System;
using Windows.UI.Xaml.Data;

namespace AvansApp.Converters
{
    public class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string status = string.Empty;
            switch(value.ToString().ToUpper())
            {
                case "D":
                    status = "Definitief";
                    break;
                case "C":
                    status = "Concept";
                    break;
                 default:
                    status = "Onbekend";
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
