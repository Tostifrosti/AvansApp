﻿using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls;

using AvansApp.Helpers;
using AvansApp.Services.Pages;
using AvansApp.Services;

namespace AvansApp.ViewModels.Pages
{
    public class EmployeePageViewModel : ViewModelBase
    {
        public NavigationService NavigationService
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<NavigationService>();
            }
        }
        public ObservableCollection<EmployeeVM> Items { get; private set; }
        
        private EmployeeVM _selected;
        public EmployeeVM Selected
        {
            get { return _selected; }
            set { Set(ref _selected, value); }
        }
        
        private bool SearchBoxChanged; // TODO
        //private Flyout SearchFlyout { get; set; } // TODO
        private string _searchBoxText;
        public string SearchBoxText
        {
            get { return _searchBoxText; }
            set { Set(ref _searchBoxText, value); }
        }

        public ICommand OnSearchButtonClickCommand { get; private set; }
        public ICommand OnItemClickCommand { get; private set; }
        public ICommand OnKeyDownCommand { get; private set; }

        private EmployeeService Service { get; set; }
        public EmployeePageViewModel()
        {
            Items = new ObservableCollection<EmployeeVM>();

            OnSearchButtonClickCommand = new RelayCommand<ItemClickEventArgs>(OnSearchButtonClick);
            OnItemClickCommand = new RelayCommand<ItemClickEventArgs>(OnItemClick);
            OnKeyDownCommand = new RelayCommand<KeyRoutedEventArgs>(OnKeyDown);

            SearchBoxChanged = false;
            Service = new EmployeeService();
            
            //this.Loaded += EmployeesPage_Loaded;
        }
        public void Initialize()
        {
            MainPageViewModel.SetPageTitle("Shell_EmployeesPage".GetLocalized());
        }

        /*private void EmployeesPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            SearchFlyout = Resources["SearchFlyout"] as Flyout;
            SearchFlyout.Placement = Windows.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.Bottom;
            SearchFlyout.Hide();
        }*/
        

        private async void OnSearchButtonClick(ItemClickEventArgs args)
        {
            // Get Search Box Input.
            if (SearchBoxChanged)
            {
                SearchBoxChanged = false;
                if (!string.IsNullOrEmpty(SearchBoxText) && !string.IsNullOrWhiteSpace(SearchBoxText))
                {
                    if (SearchBoxText.Length >= 3)
                    {
                        Items.Clear();

                        List<EmployeeVM> data = await Service.GetEmployees(SearchBoxText);
                        foreach (var item in data)
                        {
                            Items.Add(item);
                        }
                        //SearchFlyout.Hide();
                    }
                    else
                    {
                        // Notify User that the input must be longer than 3 or more characters!
                        //SearchFlyout.ShowAt(SearchBox);
                    }

                }
                else
                {
                    // Empty List
                    Items.Clear();
                    //SearchFlyout.Hide();
                }
            }
        }

        private async void OnItemClick(ItemClickEventArgs args)
        {
            EmployeeVM item = args?.ClickedItem as EmployeeVM;
            //main.PageTitle = SelectedEmployee.DisplayName;

            if (item != null)
            {
                await Service.GetEmployeeImage(item);

                // Go To page
                NavigationService.Navigate(typeof(EmployeeSinglePageViewModel).FullName, item);
            }
        }

        private void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (e != null && e.KeyStatus.RepeatCount == 1)
            {
                if (e.Key == Windows.System.VirtualKey.Enter)
                {
                    OnSearchButtonClick(null);
                }
                else
                {
                    SearchBoxChanged = true;
                }
                e.Handled = false;
            }
        }
    }
}
