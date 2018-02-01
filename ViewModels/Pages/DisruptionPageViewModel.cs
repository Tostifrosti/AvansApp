using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

using AvansApp.Helpers;
using AvansApp.Services;
using AvansApp.Services.Pages;
using System;
using System.Collections.Generic;

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
        private bool _hasNoResult;
        public bool HasNoResult
        {
            get { return _hasNoResult; }
            set { Set(ref _hasNoResult, value); }
        }
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { Set(ref _isLoading, value); }
        }
        public DisruptionService Service { get; private set; }
        public ICommand ItemClickCommand { get; private set; }
        public ICommand RefreshItemsCommand { get; private set; }
        private DateTime refreshTime;
        private bool _isPage;

        public DisruptionPageViewModel()
        {
            Items = new ObservableCollection<DisruptionItemVM>();
            Service = new DisruptionService();
            ItemClickCommand = new RelayCommand<ItemClickEventArgs>(OnItemClick);
            RefreshItemsCommand = new RelayCommand(OnRefreshItems);
            refreshTime = DateTime.Now;
            _isPage = false;
        }
        public void Initialize(bool isPage)
        {
            _isPage = isPage;
            MainPageViewModel.SetPageTitle("Shell_DisruptionsPage".GetLocalized());
        }

        public async Task LoadDataAsync()
        {
            if(Items.Count <= 0 || refreshTime <= DateTime.Now.AddMinutes(-5))
            {
                refreshTime = DateTime.Now;
                IsLoading = true;
                HasNoResult = false;
                Items.Clear();

                var data = await Service.GetDisruptions();
                data = data.OrderByDescending(d => d.PublicationDate).ToList();

                foreach (var item in data)
                {
                    Items.Add(item);
                }
                Selected = Items.FirstOrDefault();
                IsLoading = false;
                HasNoResult = Items.Count <= 0;
            }
        }

        private void OnItemClick(ItemClickEventArgs args)
        {
            DisruptionItemVM item = args?.ClickedItem as DisruptionItemVM;
            if (item != null && item.Description != null)
            {
                if (_isPage)
                    NavigationService.NavigateToPage(typeof(DisruptionSinglePageViewModel).FullName, new Views.DisruptionsSinglePage(), item);
                else
                    NavigationService.Navigate(typeof(DisruptionSinglePageViewModel).FullName, item);
            }
        }

        private async void OnRefreshItems()
        {
            // Pull-to-refresh will be available every 30 secs
            if (refreshTime <= DateTime.Now.AddSeconds(-30))
            {
                refreshTime = DateTime.Now;

                var data = await Service.GetDisruptions();
                data = new List<DisruptionItemVM>(data.OrderByDescending(d => d.PublicationDate));

                var list = new List<DisruptionItemVM>();

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
