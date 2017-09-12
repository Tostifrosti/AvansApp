using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace AvansApp.Converters
{
    public class DigitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string result = (value != null) ? value.ToString() : string.Empty;
            result = result.Replace(",", ".");
            
            if(double.TryParse(result, NumberStyles.Any, CultureInfo.InvariantCulture, out double output))
            {
                output = Math.Round(output, 2);
                return output.ToString();
            }
            
            return (result.Length > 3) ? result.Substring(0, 3) : result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
