using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using AvansApp.ViewModels;
using AvansApp.ViewModels.Pages;

namespace AvansApp.Views
{
    public sealed partial class EmployeesSinglePage : Page
    {
        
        private EmployeeSinglePageViewModel ViewModel
        {
            get { return DataContext as EmployeeSinglePageViewModel; }
        }
        public EmployeesSinglePage()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.Item = e.Parameter as EmployeeVM;
            ViewModel.Initialize();
            ViewModel.SetEmployeeImage();
        }

    }
}
