using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using AvansApp.ViewModels.Pages;

namespace AvansApp.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPageViewModel ViewModel
        {
            get { return DataContext as SettingsPageViewModel; }
        }
        public SettingsPage()
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
