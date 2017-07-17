using System;
using Windows.UI;
using Windows.UI.Xaml.Data;

namespace AvansApp.Converters
{
    public class ColorConverter : IValueConverter
    {
        private Color Red;
        private Color Green;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Red = Colors.Red;
            Green = Colors.Green;

            Color color;
            string resultaat = value.ToString();
            resultaat = resultaat.Replace(",", ".");
            double result = 0;

            if (double.TryParse(resultaat, out result))
            {
                if (result >= 5.5)
                    color = Green;
                else
                    color = Red;
            }
            else
            {
                if (resultaat.ToUpper().Equals("NVD") || resultaat.ToUpper().Equals("O"))
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
