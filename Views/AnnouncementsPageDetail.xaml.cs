using AvansApp.ViewModels;
using AvansApp.ViewModels.Pages;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace AvansApp.Views
{
    public sealed partial class AnnouncementsPageDetail : Page
    {
        private AnnouncementPageDetailViewModel ViewModel
        {
            get { return DataContext as AnnouncementPageDetailViewModel; }
        }
        public AnnouncementsPageDetail()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.Item = e.Parameter as AnnouncementVM;
        }
    }
}
