using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AvansApp.Helpers
{
    public class Untils
    {
        public static readonly DependencyProperty SourceStringProperty = DependencyProperty.RegisterAttached("SourceString", typeof(string), typeof(Untils), new PropertyMetadata("", OnSourceStringChanged));

        public static string GetSourceString(DependencyObject obj)
        {
            return obj.GetValue(SourceStringProperty).ToString();
        }

        public static void SetSourceString(DependencyObject obj, string value)
        {
            obj.SetValue(SourceStringProperty, value);
        }

        private static void OnSourceStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WebView wv)
            {
                wv.NavigateToString(e.NewValue.ToString());
            }
        }
    }
}
