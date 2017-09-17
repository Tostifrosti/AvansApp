using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace AvansApp.Converters
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isVisible = false;
            bool.TryParse((value != null) ? value.ToString() : string.Empty, out isVisible);

            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Visibility? isVisible = (Visibility)value;
            
            return (isVisible != null && isVisible.Value == Visibility.Visible) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
