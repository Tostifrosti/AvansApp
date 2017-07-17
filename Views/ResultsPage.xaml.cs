using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using AvansApp.ViewModels.Pages;

namespace AvansApp.Views
{
    public sealed partial class ResultsPage : Page
    {
        private ResultPageViewModel ViewModel
        {
            get { return DataContext as ResultPageViewModel; }
        }

        public ResultsPage()
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
