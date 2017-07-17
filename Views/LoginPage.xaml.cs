using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using AvansApp.ViewModels.Pages;

namespace AvansApp.Views
{
    public sealed partial class LoginPage : Page
    {
        private LoginPageViewModel ViewModel
        {
            get { return DataContext as LoginPageViewModel; }
        }

        public LoginPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.Initialize();
        }
    }
}
