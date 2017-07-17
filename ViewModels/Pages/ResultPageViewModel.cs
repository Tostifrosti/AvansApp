﻿using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using AvansApp.Helpers;
using AvansApp.Services.Pages;
using AvansApp.Services;

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
        public ICommand StateChangedCommand { get; private set; }
        public ICommand ItemClickCommand { get; private set; }

        public ResultPageViewModel()
        {
            Service = new ResultService();
            Items = new ObservableCollection<ResultVM>();
            ItemClickCommand = new RelayCommand<ItemClickEventArgs>(OnItemClick);
            StateChangedCommand = new RelayCommand<VisualStateChangedEventArgs>(OnStateChanged);
        }
        public void Initialize()
        {
            MainPageViewModel.SetPageTitle("Shell_ResultsPage".GetLocalized());
        }
        public async Task LoadDataAsync(VisualState currentState)
        {
            _currentState = currentState;
            if (Items.Count <= 0)
            {
                var data = await Service.GetResults();
                //data = data.OrderByDescending(d => d.ToetsDatum).ToList();
                data = data.OrderByDescending(d => d.MutatieDatum).ToList();

                foreach (var item in data)
                {
                    Items.Add(item);
                }
                Selected = Items.FirstOrDefault();
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
    }
}
