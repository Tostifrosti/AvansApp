using AvansApp.ViewModels.Pages;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace AvansApp.Views
{
    public sealed partial class EmployeesPage : Page
    {
        private EmployeePageViewModel ViewModel
        {
            get { return DataContext as EmployeePageViewModel; }
        }
        public EmployeesPage()
        {
            InitializeComponent();   
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Initialize();
        }
    }
}
