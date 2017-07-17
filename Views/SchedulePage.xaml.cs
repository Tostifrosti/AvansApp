using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using AvansApp.ViewModels.Pages;

namespace AvansApp.Views
{
    public sealed partial class SchedulePage : Page
    {
        private SchedulePageViewModel ViewModel {
            get { return DataContext as SchedulePageViewModel; }
        }
        public SchedulePage()
        {
            InitializeComponent();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Initialize();
            await ViewModel.LoadDataAsync();
        }
        
    }
}
