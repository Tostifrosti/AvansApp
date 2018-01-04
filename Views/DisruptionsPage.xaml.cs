using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using AvansApp.ViewModels.Pages;

namespace AvansApp.Views
{
    public sealed partial class DisruptionsPage : Page
    {
        public DisruptionPageViewModel ViewModel
        {
            get { return DataContext as DisruptionPageViewModel; }
        }
        
        public DisruptionsPage()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Initialize((e.Parameter != null && bool.Parse(e.Parameter.ToString()) == true));
            await ViewModel.LoadDataAsync();
        }
    }
}
