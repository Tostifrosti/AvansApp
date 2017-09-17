using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace AvansApp.Converters
{
    public class InverseVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return Visibility.Collapsed;

            bool isVisible = false;
            bool.TryParse(value.ToString(), out isVisible);

            return isVisible ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Visibility? isVisible = (Visibility)value;

            return (isVisible != null && isVisible.Value == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
