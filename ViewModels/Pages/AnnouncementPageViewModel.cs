using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using AvansApp.Helpers;
using AvansApp.Services;
using AvansApp.Services.Pages;

namespace AvansApp.ViewModels.Pages
{
    public class AnnouncementPageViewModel : ViewModelBase
    {
        public NavigationService NavigationService
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<NavigationService>();
            }
        }
        private bool _isLoading;
        public bool IsLoading {
            get { return _isLoading; }
            set { Set(ref _isLoading, value); }
        }
        private bool _hasNoResult;
        public bool HasNoResult {
            get { return _hasNoResult; }
            set { Set(ref _hasNoResult, value); }
        }
        private VisualState _currentState;

        private AnnouncementVM _selected;
        public AnnouncementVM Selected
        {
            get { return _selected; }
            set { Set(ref _selected, value); }
        }

        public ICommand ItemClickCommand { get; private set; }
        public ICommand StateChangedCommand { get; private set; }

        public ObservableCollection<AnnouncementVM> Items { get; private set; }
        public AnnouncementService Service { get; private set; }

        public AnnouncementPageViewModel()
        {
            IsLoading = true;
            Service = Singleton<AnnouncementService>.Instance;
            Items = new ObservableCollection<AnnouncementVM>();
            ItemClickCommand = new RelayCommand<ItemClickEventArgs>(OnItemClick);
            StateChangedCommand = new RelayCommand<VisualStateChangedEventArgs>(OnStateChanged);
        }
        public void Initialize()
        {
            MainPageViewModel.SetPageTitle("Shell_AnnouncementsPage".GetLocalized());
        }
        public async Task LoadDataAsync(VisualState currentState)
        {
            _currentState = currentState;
            if (Items.Count <= 0)
            {
                IsLoading = true;
                HasNoResult = false;
                var data = await Service.GetAnnouncements();
                data = new List<AnnouncementVM>(data.OrderByDescending(d => d.DateTime));

                foreach (var item in data)
                {
                    Items.Add(item);
                }
                Selected = Items.FirstOrDefault();
                IsLoading = false;
                HasNoResult = Items.Count <= 0;
            }
        }

        private void OnStateChanged(VisualStateChangedEventArgs args)
        {
            _currentState = args.NewState;
        }

        private void OnItemClick(ItemClickEventArgs args)
        {
            AnnouncementVM item = args?.ClickedItem as AnnouncementVM;
            if (item != null && item.Message != null)
            {
                if (_currentState.Name == "NarrowState")
                {
                    NavigationService.Navigate(typeof(AnnouncementPageDetailViewModel).FullName, item);
                }
                else
                {
                    Selected = item;
                }
            }
        }
    }
}
