using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using AvansApp.ViewModels.Pages;

namespace AvansApp.Views
{
    public sealed partial class MyProfilePage : Page
    {
        private ProfilePageViewModel ViewModel
        {
            get { return DataContext as ProfilePageViewModel; }
        }
        public MyProfilePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.Initialize(e);
        }
        
    }
}
