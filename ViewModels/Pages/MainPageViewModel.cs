using System;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Foundation.Metadata;

using AvansApp.Models;
using AvansApp.Helpers;
using AvansApp.Services;
using AvansApp.Services.Pages;

namespace AvansApp.ViewModels.Pages
{
    public class MainPageViewModel : ViewModelBase
    {
        private ProfileVM _user;
        public ProfileVM User { get { return _user; } set { Set(ref _user, value); } }
        private string _pageTitle;
        public string PageTitle { get { return _pageTitle; } private set { Set(ref _pageTitle, value); } }
        private const string PanoramicStateName = "PanoramicState";
        private const string WideStateName = "WideState";
        private const string NarrowStateName = "NarrowState";
        private bool _isHeaderVisible;
        public bool IsHeaderVisible { get { return _isHeaderVisible; } set { Set(ref _isHeaderVisible, value); } }
        private ImageSource _profileImage;
        public ImageSource ProfileImage { get { return _profileImage; } set { Set(ref _profileImage, value); } }
        public ICommand ShowMyProfileCommand { get; set; }
        public static MainPageViewModel Instance { get; private set; }

        public MainPageViewModel()
        {
            Instance = this;
            IsHeaderVisible = true;
            SetPageTitle("Avans");
            ProfileImage = new BitmapImage(new Uri("ms-appx:///Assets/StoreLogo.png"));
            ShowMyProfileCommand = new RelayCommand(ShowProfilePage);
            GetUserAsync();
            SetStatusBarColor(Color.FromArgb(255, 198, 0, 42), Colors.White); // Change color of Statusbar

            /*if (!OAuth.GetInstance().Client.CheckTokenExists("Settlement"))
            {
                OAuth.GetInstance().Client.AddToVault("Settlement", "Settlement", "onderwijsboulevard");
            }*/
        }

        public void Initialize(Frame frame)
        {
            NavigationService.Frame = frame;
            NavigationService.Frame.Navigated += NavigationService_Navigated;
            PopulateNavItems();
        }

        private async void GetUserAsync()
        {
            User = await Singleton<ProfileService>.Instance.GetUserAsync();
            ProfileImage = User.EmployeeImage.bitmap;
        }
        private void ShowProfilePage()
        {
            IsPaneOpen = false;
            NavigationService.Navigate(typeof(ProfilePageViewModel).FullName);
        }
        private async void SetStatusBarColor(Color background, Color foreground)
        {
            // Turn on SystemTray for mobile
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                // Added Reference: Windows Desktop Extensions for the UWP
                // Added Reference: Windows Mobile Extensions for the UWP
                Windows.UI.ViewManagement.StatusBar statusbar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                if (statusbar != null)
                {
                    await statusbar.ShowAsync();
                    statusbar.BackgroundOpacity = 1;
                    statusbar.BackgroundColor = background;
                    statusbar.ForegroundColor = foreground;
                }
            }
        }
        
        public NavigationService NavigationService
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<NavigationService>();
            }
        }

        private bool _isPaneOpen;
        public bool IsPaneOpen
        {
            get { return _isPaneOpen; }
            set { Set(ref _isPaneOpen, value); }
        }

        private SplitViewDisplayMode _displayMode = SplitViewDisplayMode.Overlay; //SplitViewDisplayMode.CompactInline;
        public SplitViewDisplayMode DisplayMode
        {
            get { return _displayMode; }
            set { Set(ref _displayMode, value); }
        }

        private object _lastSelectedItem;

        private ObservableCollection<ShellNavigationItem> _primaryItems = new ObservableCollection<ShellNavigationItem>();
        public ObservableCollection<ShellNavigationItem> PrimaryItems
        {
            get { return _primaryItems; }
            set { Set(ref _primaryItems, value); }
        }

        private ObservableCollection<ShellNavigationItem> _secondaryItems = new ObservableCollection<ShellNavigationItem>();
        public ObservableCollection<ShellNavigationItem> SecondaryItems
        {
            get { return _secondaryItems; }
            set { Set(ref _secondaryItems, value); }
        }

        private ICommand _openPaneCommand;
        public ICommand OpenPaneCommand
        {
            get
            {
                if (_openPaneCommand == null)
                {
                    _openPaneCommand = new RelayCommand(() => IsPaneOpen = !_isPaneOpen);
                }

                return _openPaneCommand;
            }
        }

        private ICommand _itemSelected;
        public ICommand ItemSelectedCommand
        {
            get
            {
                if (_itemSelected == null)
                {
                    _itemSelected = new RelayCommand<ShellNavigationItem>(ItemSelected);
                }

                return _itemSelected;
            }
        }

        private ICommand _stateChangedCommand;
        public ICommand StateChangedCommand
        {
            get
            {
                if (_stateChangedCommand == null)
                {
                    _stateChangedCommand = new RelayCommand<VisualStateChangedEventArgs>(OnStateChanged);
                }

                return _stateChangedCommand;
            }
        }

        private void OnStateChanged(VisualStateChangedEventArgs args)
        {
            switch (args.NewState.Name)
            {
                case PanoramicStateName:
                    DisplayMode = SplitViewDisplayMode.Overlay; //SplitViewDisplayMode.CompactInline;
                    break;
                case WideStateName:
                    DisplayMode = SplitViewDisplayMode.Overlay; //SplitViewDisplayMode.CompactInline;
                    IsPaneOpen = false;
                    break;
                case NarrowStateName:
                    DisplayMode = SplitViewDisplayMode.Overlay; //SplitViewDisplayMode.Overlay;
                    IsPaneOpen = false;
                    break;
                default:
                    break;
            }
        }
        
        private void PopulateNavItems()
        {
            _primaryItems.Clear();
            _secondaryItems.Clear();

            // More on Segoe UI Symbol icons: https://docs.microsoft.com/windows/uwp/style/segoe-ui-symbol-font
            // Edit String/en-US/Resources.resw: Add a menu item title for each page
            _primaryItems.Add(new ShellNavigationItem("Shell_SchedulePage".GetLocalized(), Symbol.Calendar, typeof(SchedulePageViewModel).FullName));
            _primaryItems.Add(new ShellNavigationItem("Shell_AnnouncementsPage".GetLocalized(), Symbol.Message, typeof(AnnouncementPageViewModel).FullName));
            _primaryItems.Add(new ShellNavigationItem("Shell_ResultsPage".GetLocalized(), Symbol.Bullets, typeof(ResultPageViewModel).FullName));
            _primaryItems.Add(new ShellNavigationItem("Shell_ExamsPage".GetLocalized(), Symbol.Copy, typeof(ExamPageViewModel).FullName));
            _primaryItems.Add(new ShellNavigationItem("Shell_ClassroomAvailabilityPage".GetLocalized(), Symbol.Clock, typeof(ClassroomAvailabilityPageViewModel).FullName));
            _primaryItems.Add(new ShellNavigationItem("Shell_EmployeesPage".GetLocalized(), Symbol.People, typeof(EmployeePageViewModel).FullName));
            _primaryItems.Add(new ShellNavigationItem("Shell_DisruptionsPage".GetLocalized(), Symbol.Repair, typeof(DisruptionPageViewModel).FullName));

            _secondaryItems.Add(new ShellNavigationItem("Shell_SettingsPage".GetLocalized(), Symbol.Setting, typeof(SettingsPageViewModel).FullName));
        }

        private void ItemSelected(ShellNavigationItem e)
        {
            if (DisplayMode == SplitViewDisplayMode.CompactOverlay || DisplayMode == SplitViewDisplayMode.Overlay)
            {
                IsPaneOpen = false;
            }
            Navigate(e);
        }

        private void NavigationService_Navigated(object sender, NavigationEventArgs e)
        {
            if (e != null)
            {
                var vm = NavigationService.GetNameOfRegisteredPage(e.SourcePageType);
                var item = PrimaryItems?.FirstOrDefault(i => i.ViewModelName == vm);
                if (item == null)
                {
                    item = SecondaryItems?.FirstOrDefault(i => i.ViewModelName == vm);
                }

                if (item != null)
                {
                    ChangeSelected(_lastSelectedItem, item);
                    _lastSelectedItem = item;
                }
            }
        }

        private void ChangeSelected(object oldValue, object newValue)
        {
            if (oldValue != null)
            {
                (oldValue as ShellNavigationItem).IsSelected = false;
            }
            if (newValue != null)
            {
                (newValue as ShellNavigationItem).IsSelected = true;
            }
        }

        private void Navigate(object item)
        {
            if (item is ShellNavigationItem navigationItem)
            {
                NavigationService.Navigate(navigationItem.ViewModelName);
            }
        }
        public static void SetPageTitle(string title)
        {
            if (Instance != null)
                Instance.PageTitle = title;
        }
    }
}
