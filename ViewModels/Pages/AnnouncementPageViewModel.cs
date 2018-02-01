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
using System;

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
        public ICommand RefreshItemsCommand { get; private set; }
        public ObservableCollection<AnnouncementVM> Items { get; private set; }
        public AnnouncementService Service { get; private set; }
        private DateTime refreshTime;

        public AnnouncementPageViewModel()
        {
            IsLoading = true;
            Service = Singleton<AnnouncementService>.Instance;
            Items = new ObservableCollection<AnnouncementVM>();
            ItemClickCommand = new RelayCommand<ItemClickEventArgs>(OnItemClick);
            StateChangedCommand = new RelayCommand<VisualStateChangedEventArgs>(OnStateChanged);
            RefreshItemsCommand = new RelayCommand(OnRefreshItems);
            refreshTime = DateTime.Now;
        }
        public void Initialize()
        {
            MainPageViewModel.SetPageTitle("Shell_AnnouncementsPage".GetLocalized());
        }
        public async Task LoadDataAsync(VisualState currentState)
        {
            _currentState = currentState;
            if (Items.Count <= 0 || refreshTime <= DateTime.Now.AddMinutes(-5))
            {
                refreshTime = DateTime.Now;
                IsLoading = true;
                HasNoResult = false;
                Items.Clear();

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

        private async void OnRefreshItems()
        {
            // Pull-to-refresh will be available every 30 secs
            if (refreshTime <= DateTime.Now.AddSeconds(-30))
            {
                refreshTime = DateTime.Now;
                
                var data = await Service.GetAnnouncements();
                data = new List<AnnouncementVM>(data.OrderByDescending(d => d.DateTime));

                var list = new List<AnnouncementVM>();
                
                for (int i=0; i < data.Count; i++)
                {
                    bool isFound = false;
                    for (int j = 0; j < Items.Count; j++)
                    {
                        if (Service.Compare(data[i], Items[j]) == true)
                        {
                            isFound = true;
                        }
                    }
                    if (!isFound)
                        list.Add(data[i]);
                }
                
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        Items.Insert(0, item);
                    }
                    Selected = Items.FirstOrDefault();
                }
                HasNoResult = Items.Count <= 0;
            }
        }

    }
}
