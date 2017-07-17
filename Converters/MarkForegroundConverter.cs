using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace AvansApp.Converters
{
    public class MarkForegroundConverter : IValueConverter
    {
        private SolidColorBrush Red;
        private SolidColorBrush Green;
        private SolidColorBrush Black;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Red = new SolidColorBrush(Colors.Red);
            Green = new SolidColorBrush(Colors.Green);
            Black = new SolidColorBrush(Colors.Black);

            SolidColorBrush color;
            string resultaat = value.ToString();
            resultaat = resultaat.Replace(",", ".");
            double result = 0;

            if (double.TryParse(resultaat, out result))
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
