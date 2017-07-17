using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using AvansApp.ViewModels.Pages;

namespace AvansApp.Views
{
    public sealed partial class ExamsPage : Page
    {
        private ExamPageViewModel ViewModel
        {
            get { return DataContext as ExamPageViewModel; }
        }

        public ExamsPage()
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
