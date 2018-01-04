using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using AvansApp.Helpers;
using AvansApp.Services.Pages;
using AvansApp.Services;
using System;
using System.Collections.Generic;

namespace AvansApp.ViewModels.Pages
{
    public class ResultPageViewModel : ViewModelBase
    {
        public NavigationService NavigationService
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<NavigationService>();
            }
        }
        private ResultService Service { get; set; }
        public ObservableCollection<ResultVM> Items { get; private set; }
        private VisualState _currentState;
        private ResultVM _selected;
        public ResultVM Selected {
            get { return _selected; }
            set { Set(ref _selected, value); }
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
        public ICommand StateChangedCommand { get; private set; }
        public ICommand ItemClickCommand { get; private set; }
        public ICommand RefreshItemsCommand { get; private set; }
        private DateTime refreshTime;

        public ResultPageViewModel()
        {
            Service = Singleton<ResultService>.Instance;
            Items = new ObservableCollection<ResultVM>();
            ItemClickCommand = new RelayCommand<ItemClickEventArgs>(OnItemClick);
            StateChangedCommand = new RelayCommand<VisualStateChangedEventArgs>(OnStateChanged);
            RefreshItemsCommand = new RelayCommand(OnRefreshItems);
            refreshTime = DateTime.Now;
        }
        public void Initialize()
        {
            MainPageViewModel.SetPageTitle("Shell_ResultsPage".GetLocalized());
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

                var data = await Service.GetResults();
                data = data.OrderByDescending(d => d.MutatieDatum).ToList();

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
            ResultVM item = args?.ClickedItem as ResultVM;
            if (item != null)
            {
                if (_currentState.Name == "NarrowState")
                {
                    NavigationService.Navigate(typeof(ResultPageDetailViewModel).FullName, item);
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

                var data = await Service.GetResults();
                data = new List<ResultVM>(data.OrderByDescending(d => d.MutatieDatum));

                var list = new List<ResultVM>();

                for (int i = 0; i < data.Count; i++)
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
