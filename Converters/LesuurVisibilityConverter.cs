using System;
using Windows.UI.Xaml.Data;

namespace AvansApp.Converters
{
    public class LesuurVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool enabled = false;
            int lesuur = -1;
            int duration = 45; // minutes
            DateTime startDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 45, 0);

            /*
            1	08.45 - 9.30	 	9	15.40 - 16.25
            2	09.35 - 10.20	 	10	16.30 - 17.15
 	            korte pauze	 	    11	17.15 - 18.00**
            3	10.35 - 11.20	 	12	18.00 - 18.45**
            4	11.25 - 12.10	 	13	18.45 - 19.30
            5	12.15 - 13.00*	 	14	19.30 - 20.15
            6	13.00 - 13.45*	 	 	korte pauze
            7	13.50 - 14.35	 	15	20.30 - 21.15
            8	14.40 - 15.25	 	16	21.15 - 22.00

            *	tevens middagpauze
            **	tevens avondpauze
            
            */

            if (int.TryParse((value != null) ? value.ToString() : string.Empty, out lesuur))
            {
                if (lesuur > 0)
                {
                    int additionalMinutes = 0;
                    additionalMinutes += (((lesuur >= 2) ? (lesuur - 1) : 0) * 5); // Looppauze
                    additionalMinutes += (lesuur > 2) ? 10 : 0; // Ochtend pauze
                    additionalMinutes += (lesuur > 8) ? 10 : 0; // Middag pauze
                    additionalMinutes += (lesuur > 14) ? 10 : 0; // Avond pauze

                    additionalMinutes -= (lesuur > 5) ? 5 : 0;
                    additionalMinutes -= (lesuur > 10) ? 5 : 0;
                    additionalMinutes -= (lesuur > 11) ? 5 : 0;
                    additionalMinutes -= (lesuur > 12) ? 5 : 0;
                    additionalMinutes -= (lesuur > 13) ? 5 : 0;
                    additionalMinutes -= (lesuur > 15) ? 5 : 0;

                    DateTime currentLesuur = startDay.AddMinutes(((lesuur * duration) + additionalMinutes));

                    int minutes = (int)Math.Floor((currentLesuur - DateTime.Now).TotalMinutes);

                    if (minutes > 0) // Is de dag al begonnen?
                    {
                        int diff = (minutes / duration);

                        if (diff < 0) // Is het nog binnen 1 lesuur?
                            enabled = false;
                        else
                            enabled = true;
                    }
                    else
                        enabled = false;
                }
                else
                {
                    int beforeFirstLesuurDiff = (int)Math.Floor((startDay - DateTime.Now).TotalMinutes);
                    if (beforeFirstLesuurDiff >= 0)
                        enabled = true;
                    else
                        enabled = false;
                }
            }



            return (enabled == true) ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
