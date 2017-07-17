using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using AvansApp.ViewModels;
using AvansApp.ViewModels.Pages;

namespace AvansApp.Views
{
    public sealed partial class ResultsPageDetail : Page
    {
        private ResultPageDetailViewModel ViewModel
        {
            get { return DataContext as ResultPageDetailViewModel; }
        }

        public ResultsPageDetail()
        {
            InitializeComponent();
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.Item = e.Parameter as ResultVM;
            ViewModel.Initialize();
        }
    }
}
