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
        private ClassroomAvailabilityService Service { get; set; }
        public ICommand OnSearchButtonClickCommand { get; private set; }
        public ICommand OnItemClickCommand { get; private set; }
        public ICommand OnKeyDownCommand { get; private set; }
        public ObservableCollection<ClassroomAvailabilityVM> Items { get; private set; }
        public ObservableCollection<ClassroomVM> Classrooms { get; private set; }
        private ClassroomVM Selected { get; set; }

        public ClassroomAvailabilityPageViewModel()
        {
            Service = new ClassroomAvailabilityService();
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
                        Items.Clear();
                    }
                }
                else
                {
                    // Empty List
                    Items.Clear();
                }
            }
        }
        
        private void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (e != null && e.KeyStatus.RepeatCount == 1)
            {
                if (e.Key == Windows.System.VirtualKey.Enter)
                {
                    //SearchButton.Focus(FocusState.Pointer); // Lowers Virtual Keyboard
                    OnSearchButtonClick(null);
                }
                else
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
                //main.PageTitle = SelectedClassroom.Classroom;

                NavigationService.Navigate(typeof(ClassroomAvailabilityPageDetailViewModel).FullName, item);
            }
        }
    }
}
