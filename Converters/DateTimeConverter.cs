using System;
using Windows.UI.Xaml.Data;

namespace AvansApp.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return string.Empty;
            string date = string.Empty;
            DateTime now = DateTime.Now;
            DateTime dt = DateTime.Parse(value.ToString());

            bool isShortened = (parameter != null && parameter.ToString().ToLower().Equals("true")) ? true : false;

            // dd-MM-yyyy HH:mm:ss
            if (dt.Year == now.Year &&
                dt.Month == now.Month &&
                dt.Day == now.Day)
            {
                date = dt.ToString("HH:mm");
            } else
            {
                if (isShortened)
                    date = dt.ToString("dd MMM");
                else
                    date = dt.ToString("dd MMMM");
            }

            return date;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
