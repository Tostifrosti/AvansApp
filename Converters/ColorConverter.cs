using System;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml.Data;

namespace AvansApp.Converters
{
    public class ColorConverter : IValueConverter
    {
        private Color Red = Colors.Red;
        private Color Green = Colors.Green;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Color color;
            string resultaat = (value != null) ? value.ToString() : string.Empty;
            resultaat = resultaat.Replace(",", ".");
            double result = 0;

            if (double.TryParse(resultaat, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                if (result >= 5.5)
                    color = Green;
                else
                    color = Red;
            }
            else
            {
                if (resultaat.ToUpper().Equals("NV") || resultaat.ToUpper().Equals("NVD") || resultaat.ToUpper().Equals("O") || resultaat.ToUpper().Equals("NC")) // NV = Niet Voldaan, NC = Niet Compensabel
                    color = Red;
                else if (resultaat.ToUpper().Equals("VR") || resultaat.ToUpper().Equals("NA"))
                    color = Green;
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
