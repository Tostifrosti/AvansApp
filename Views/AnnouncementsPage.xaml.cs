using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using AvansApp.ViewModels.Pages;

namespace AvansApp.Views
{
    public sealed partial class AnnouncementsPage : Page
    {
        private AnnouncementPageViewModel ViewModel
        {
            get { return DataContext as AnnouncementPageViewModel; }
        }

        public AnnouncementsPage()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Initialize();
            await ViewModel.LoadDataAsync(WindowStates.CurrentState);
        }

    }
}
