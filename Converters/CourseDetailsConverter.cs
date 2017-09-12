using AvansApp.Helpers;
using System;
using Windows.UI.Xaml.Data;

namespace AvansApp.Converters
{
    public class CourseDetailsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string result = string.Empty;

            if(!string.IsNullOrEmpty((value != null) ? value.ToString() : string.Empty))
            {
                result = value.ToString();
                result = result.Replace(";", "\n");
            }
            else
            {
                result = "Unknown".GetLocalized();
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
