using Windows.UI.Xaml.Controls;
using AvansApp.ViewModels.Pages;
using Windows.UI.Xaml.Navigation;

namespace AvansApp.Views
{
    public sealed partial class MainPage : Page
    {
        private MainPageViewModel ViewModel
        {
            get { return DataContext as MainPageViewModel; }
        } 

        public MainPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            ViewModel.Initialize(MainFrame);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.Initialize(MainFrame);
        }
    }
}
