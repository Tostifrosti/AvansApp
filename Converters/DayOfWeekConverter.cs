using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace AvansApp.Converters
{
    public class DayOfWeekConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string day = value.ToString();
            string dayOfWeek = "";
            switch (day.ToLower())
            {
                case "monday":
                case "maandag":
                    dayOfWeek = "Maandag";
                    break;
                case "tuesday":
                case "dinsdag":
                    dayOfWeek = "Dinsdag";
                    break;
                case "wednesday":
                case "woensdag":
                    dayOfWeek = "Woensdag";
                    break;
                case "thursday":
                case "donderdag":
                    dayOfWeek = "Donderdag";
                    break;
                case "friday":
                case "vrijdag":
                    dayOfWeek = "Vrijdag";
                    break;
                case "saturday":
                case "zaterdag":
                    dayOfWeek = "Zaterdag";
                    break;
                case "sunday":
                case "zondag":
                    dayOfWeek = "Zondag";
                    break;
                default:
                    break;

            }
            return dayOfWeek;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
