using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

using AvansApp.Helpers;
using AvansApp.Services;
using AvansApp.Services.Pages;

namespace AvansApp.ViewModels.Pages
{
    public class DisruptionPageViewModel : ViewModelBase
    {
        public NavigationService NavigationService
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<NavigationService>();
            }
        }
        public ObservableCollection<DisruptionItemVM> Items { get; private set; }
        private DisruptionItemVM _selected;
        public DisruptionItemVM Selected
        {
            get { return _selected; }
            set { Set(ref _selected, value); }
        }
        public DisruptionService Service { get; private set; }
        public ICommand ItemClickCommand { get; private set; }

        public DisruptionPageViewModel()
        {
            Items = new ObservableCollection<DisruptionItemVM>();
            Service = new DisruptionService();
            ItemClickCommand = new RelayCommand<ItemClickEventArgs>(OnItemClick);
        }
        public void Initialize()
        {
            MainPageViewModel.SetPageTitle("Shell_DisruptionsPage".GetLocalized());
        }

        public async Task LoadDataAsync()
        {
            if(Items.Count <= 0)
            {
                var data = await Service.GetDisruptions();
                data = data.OrderByDescending(d => d.PublicationDate).ToList();

                foreach (var item in data)
                {
                    Items.Add(item);
                }
                Selected = Items.FirstOrDefault();
            }
        }

        

        private void OnItemClick(ItemClickEventArgs args)
        {
            DisruptionItemVM item = args?.ClickedItem as DisruptionItemVM;
            if (item != null)
            {
                NavigationService.Navigate(typeof(DisruptionSinglePageViewModel).FullName, item);
            }
        }
    }
}
