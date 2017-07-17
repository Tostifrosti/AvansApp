using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using AvansApp.ViewModels;

namespace AvansApp.Views
{
    public sealed partial class ResultsPageDetailControl : UserControl
    {
        public ResultVM MasterMenuItem
        {
            get { return GetValue(MasterMenuItemProperty) as ResultVM; }
            set { SetValue(MasterMenuItemProperty, value); }
        }

        public static DependencyProperty MasterMenuItemProperty = DependencyProperty.Register("MasterMenuItem", typeof(ResultVM), typeof(ResultsPageDetailControl), new PropertyMetadata(null));

        public ResultsPageDetailControl()
        {
            InitializeComponent();
        }
    }
}
