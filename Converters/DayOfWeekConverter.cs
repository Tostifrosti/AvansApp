using AvansApp.Helpers;
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
                    dayOfWeek = "Monday".GetLocalized();
                    break;
                case "tuesday":
                case "dinsdag":
                    dayOfWeek = "Tuesday".GetLocalized();
                    break;
                case "wednesday":
                case "woensdag":
                    dayOfWeek = "Wednesday".GetLocalized();
                    break;
                case "thursday":
                case "donderdag":
                    dayOfWeek = "Thursday".GetLocalized();
                    break;
                case "friday":
                case "vrijdag":
                    dayOfWeek = "Friday".GetLocalized();
                    break;
                case "saturday":
                case "zaterdag":
                    dayOfWeek = "Saturday".GetLocalized();
                    break;
                case "sunday":
                case "zondag":
                    dayOfWeek = "Sunday".GetLocalized();
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
