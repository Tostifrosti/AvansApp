using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using AvansApp.ViewModels;
using AvansApp.ViewModels.Pages;

namespace AvansApp.Views
{
    public sealed partial class DisruptionsSinglePage : Page
    {
        private DisruptionSinglePageViewModel ViewModel
        {
            get { return DataContext as DisruptionSinglePageViewModel; }
        }

        public DisruptionsSinglePage()
        {
            InitializeComponent();
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.Item = e.Parameter as DisruptionItemVM;
        }
    }
}
