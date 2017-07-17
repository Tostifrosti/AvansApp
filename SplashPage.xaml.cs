using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using AvansApp.ViewModels.Pages;

namespace AvansApp
{
    public sealed partial class SplashPage : Page
    {
        private SplashPageViewModel ViewModel
        {
            get { return DataContext as SplashPageViewModel; }
        }

        public SplashPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Initialize();
        }
    }
}
