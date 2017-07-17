using AvansApp.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AvansApp.Views
{
    public sealed partial class AnnouncementsPageDetailControl : UserControl
    {
        public AnnouncementVM MasterMenuItem
        {
            get { return GetValue(MasterMenuItemProperty) as AnnouncementVM; }
            set { SetValue(MasterMenuItemProperty, value); }
        }

        public static DependencyProperty MasterMenuItemProperty = DependencyProperty.Register("MasterMenuItem", typeof(AnnouncementVM), typeof(AnnouncementsPageDetailControl), new PropertyMetadata(null));

        public AnnouncementsPageDetailControl()
        {
            InitializeComponent();
        }
    }
}
