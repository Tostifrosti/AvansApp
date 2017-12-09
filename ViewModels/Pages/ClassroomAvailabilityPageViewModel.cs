using System;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls;

using AvansApp.Helpers;
using AvansApp.Services;
using AvansApp.Services.Pages;
using AvansApp.Models.Enums;

namespace AvansApp.ViewModels.Pages
{
    public class ClassroomAvailabilityPageViewModel : ViewModelBase
    {
        public NavigationService NavigationService
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<NavigationService>();
            }
        }
        
        private bool SearchBoxChanged;

        private string _searchBoxText;
        public string SearchBoxText {
            get { return _searchBoxText; }
            set { Set(ref _searchBoxText, value); }
        }
        private bool _hasNoResult;
        public bool HasNoResult {
            get { return _hasNoResult; }
            set { Set(ref _hasNoResult, value); }
        }
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { Set(ref _isLoading, value); }
        }
        private bool _notSearched;
        public bool NotSearched
        {
            get { return _notSearched; }
            set { Set(ref _notSearched, value); }
        }
        private ClassroomAvailabilityService Service { get; set; }
        public ICommand OnSearchButtonClickCommand { get; private set; }
        public ICommand OnItemClickCommand { get; private set; }
        public ICommand OnKeyDownCommand { get; private set; }
        public ObservableCollection<ClassroomAvailabilityVM> Items { get; private set; }
        public ObservableCollection<ClassroomVM> Classrooms { get; private set; }
        private ClassroomVM Selected { get; set; }

        public ClassroomAvailabilityPageViewModel()
        {
            IsLoading = false;
            HasNoResult = false;
            NotSearched = true;

            Service = Singleton<ClassroomAvailabilityService>.Instance;
            Items = new ObservableCollection<ClassroomAvailabilityVM>();
            Classrooms = new ObservableCollection<ClassroomVM>();
            SearchBoxChanged = false;

            OnSearchButtonClickCommand = new RelayCommand<ItemClickEventArgs>(OnSearchButtonClick);
            OnItemClickCommand = new RelayCommand<ItemClickEventArgs>(OnItemClick);
            OnKeyDownCommand = new RelayCommand<KeyRoutedEventArgs>(OnKeyDown);
        }

        public void Initialize()
        {
            MainPageViewModel.SetPageTitle("Shell_ClassroomAvailabilityPage".GetLocalized());
        }

        private async void OnSearchButtonClick(ItemClickEventArgs e)
        {
            // Get Search Box Input.
            if (SearchBoxChanged)
            {
                SearchBoxChanged = false;
                if (!string.IsNullOrEmpty(SearchBoxText) && !string.IsNullOrWhiteSpace(SearchBoxText))
                {
                    IsLoading = true;
                    HasNoResult = false;
                    NotSearched = false;
                    Classrooms.Clear();
                    Items.Clear();

                    List<ClassroomAvailabilityVM> data = await Service.GetClassroomAvailabilities(DateTime.Now, SearchBoxText, ClassroomAvailabilityType.ALL);
                    data = data.OrderBy(ca => ca.Classroom).ToList();

                    foreach (var item in data)
                    {
                        Items.Add(item);
                    }

                    List<ClassroomVM> crooms = Service.InitializeClassroom(data);
                    if (crooms.Count > 0)
                    {
                        foreach (var item in crooms) {
                            Classrooms.Add(item);
                        }
                    } else {
                        // Empty List
                        Classrooms.Clear();
                        Items.Clear();
                        HasNoResult = true;
                    }
                    IsLoading = false;
                }
                else
                {
                    // Empty List
                    Classrooms.Clear();
                    Items.Clear();
                    IsLoading = false;
                    HasNoResult = false;
                    NotSearched = true;
                }
            }
            else if (string.IsNullOrWhiteSpace(SearchBoxText))
            {
                // Empty List
                Classrooms.Clear();
                Items.Clear();
                IsLoading = false;
                HasNoResult = false;
                NotSearched = true;
            }
        }
        
        private void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (e != null)
            {
                if (e.KeyStatus.RepeatCount == 0 && e.Key == Windows.System.VirtualKey.Enter)
                {
                    if (DeviceTypeHelper.GetDeviceFormFactorType() != DeviceFormFactorType.Desktop)
                        LoseFocus(e.OriginalSource);
                    OnSearchButtonClick(null);
                }
                else if (e.KeyStatus.RepeatCount == 1)
                {
                    SearchBoxChanged = true;
                }
                e.Handled = false;
            }
        }

        private void OnItemClick(ItemClickEventArgs e)
        {
            ClassroomVM item = e?.ClickedItem as ClassroomVM;
            if (item != null)
            {
                NavigationService.Navigate(typeof(ClassroomAvailabilityPageDetailViewModel).FullName, item);
            }
        }

        private void LoseFocus(object sender)
        {
            var control = sender as Control;
            var isTabStop = control.IsTabStop;
            control.IsTabStop = false;
            control.IsEnabled = false;
            control.IsEnabled = true;
            control.IsTabStop = isTabStop;
        }
    }
}
