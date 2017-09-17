using System;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace AvansApp.Converters
{
    public class MarkForegroundConverter : IValueConverter
    {
        private SolidColorBrush Red = new SolidColorBrush(Colors.Red);
        private SolidColorBrush Green = new SolidColorBrush(Colors.Green);
        private SolidColorBrush Black = new SolidColorBrush(Colors.Black);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            SolidColorBrush color;
            string resultaat = (value != null) ? value.ToString() : string.Empty;
            resultaat = resultaat.Replace(",", ".");
            
            if (double.TryParse(resultaat, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                if (result >= 5.5)
                    color = Green;
                else
                    color = Red;
            } else {
                if (resultaat.ToUpper().Equals("NV") || resultaat.ToUpper().Equals("NVD") || resultaat.ToUpper().Equals("O") || resultaat.ToUpper().Equals("NC")) // NV = Niet Voldaan, NC = Niet Compensabel
                    color = Red;
                else if (resultaat.ToUpper().Equals("VR") || resultaat.ToUpper().Equals("NA"))
                    color = Black;
                else
                    color = Green;
            }
            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
