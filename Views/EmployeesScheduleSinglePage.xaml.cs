using AvansApp.ViewModels;
using AvansApp.ViewModels.Pages;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace AvansApp.Views
{
    public sealed partial class EmployeesScheduleSinglePage : Page
    {
        private EmployeeScheduleSinglePageViewModel ViewModel
        {
            get { return DataContext as EmployeeScheduleSinglePageViewModel; }
        }
        public EmployeesScheduleSinglePage()
        {
            InitializeComponent();
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.Item = e.Parameter as EmployeeVM;
            ViewModel.Initialize();
            await ViewModel.LoadDataAsync();
        }
    }
}
