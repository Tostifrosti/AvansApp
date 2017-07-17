using System.Linq;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

using AvansApp.Helpers;
using AvansApp.Services.Pages;

namespace AvansApp.ViewModels.Pages
{
    public class ExamPageViewModel : ViewModelBase
    {
        public ObservableCollection<ExamVM> Items { get; private set; } = new ObservableCollection<ExamVM>();

        private ExamVM _selected;
        public ExamVM Selected
        {
            get { return _selected; }
            set { Set(ref _selected, value); }
        }

        public ICommand OnItemClickCommand { get; private set; }

        public ExamPageViewModel()
        {
            OnItemClickCommand = new RelayCommand<ItemClickEventArgs>(OnItemClick);
        }

        public void Initialize()
        {
            MainPageViewModel.SetPageTitle("Shell_ExamsPage".GetLocalized());
        }
        public async Task LoadDataAsync()
        {
            Items.Clear();

            var service = new ExamService();
            var data = await service.GetExams();
            data = data.OrderByDescending(d => d.DateTime).ToList();

            foreach (var item in data)
            {
                Items.Add(item);
            }
            Selected = Items.FirstOrDefault();
        }

        private void OnItemClick(ItemClickEventArgs e)
        {
            // TODO
        }
    }
}
