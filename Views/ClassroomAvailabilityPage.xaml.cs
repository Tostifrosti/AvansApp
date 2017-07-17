using Windows.UI.Xaml.Controls;

using AvansApp.ViewModels.Pages;
using Windows.UI.Xaml.Navigation;

namespace AvansApp.Views
{
    public sealed partial class ClassroomAvailabilityPage : Page
    {
        public ClassroomAvailabilityPageViewModel ViewModel
        {
            get { return DataContext as ClassroomAvailabilityPageViewModel; }
        }

        public ClassroomAvailabilityPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Initialize();
        }
    }
}
