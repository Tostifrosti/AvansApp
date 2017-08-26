using AvansApp.Helpers;
using System;
using Windows.UI.Xaml.Data;

namespace AvansApp.Converters
{
    public class LesuurConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int lesuur;
            string result = string.Empty;

            if (!int.TryParse(value.ToString(), out lesuur))
                lesuur = -1;

            string[] roostertijden = {
                "08.00 - 08.45",
                "08.45 - 09.30",
                "09.35 - 10.20",
                "10.35 - 11.20",
                "11.25 - 12.10",
                "12.15 - 13.00",
                "13.00 - 13.45",
                "13.50 - 14.35",
                "14.40 - 15.25",
                "15.40 - 16.25",
                "16.30 - 17.15",
                "17.15 - 18.00",
                "18.00 – 18.45",
                "18.45 – 19.30",
                "19.30 – 20.15",
                "20.30 – 21.15",
                "21.15 – 22.00"
            };

            if (lesuur >= 0 && lesuur < roostertijden.Length)
                result = roostertijden[lesuur];
            else
                result = "Unknown".GetLocalized();

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
