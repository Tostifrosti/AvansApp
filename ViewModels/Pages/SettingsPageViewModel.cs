using System;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Contacts;
using Windows.Foundation.Metadata;
using Windows.UI.Popups;
using Windows.UI.Xaml.Input;

using AvansApp.Services;
using AvansApp.Helpers;
using AvansApp.Services.Pages;

namespace AvansApp.ViewModels.Pages
{
    public class SettingsPageViewModel : ViewModelBase
    {
        public NavigationService NavigationService
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<NavigationService>();
            }
        }
        private bool SearchBoxChanged;
        private bool _isLightThemeEnabled;
        public bool IsLightThemeEnabled
        {
            get { return _isLightThemeEnabled; }
            set { Set(ref _isLightThemeEnabled, value); }
        }
        private bool _isScheduleWithoutBlanksEnabled;
        public bool IsScheduleWithoutBlanksEnabled
        {
            get { return _isScheduleWithoutBlanksEnabled; }
            set {
                Set(ref _isScheduleWithoutBlanksEnabled, value);
                Service.SaveScheduleBlanks(IsScheduleWithoutBlanksEnabled);
            }
        }
        private bool _isAnnouncementNotificationEnabled;
        public bool IsAnnouncementNotificationEnabled
        {
            get { return _isAnnouncementNotificationEnabled; }
            set
            {
                Set(ref _isAnnouncementNotificationEnabled, value);
                Service.SaveNotificationSetting(IsAnnouncementNotificationEnabled, NotificationSettingType.Announcement);
            }
        }
        private bool _isDisruptionNotificationEnabled;
        public bool IsDisruptionNotificationEnabled {
            get { return _isDisruptionNotificationEnabled; }
            set {
                Set(ref _isDisruptionNotificationEnabled, value);
                Service.SaveNotificationSetting(IsDisruptionNotificationEnabled, NotificationSettingType.Disruption);
            }
        }
        private bool _isResultNotificationEnabled;
        public bool IsResultNotificationEnabled
        {
            get { return _isResultNotificationEnabled; }
            set {
                Set(ref _isResultNotificationEnabled, value);
                Service.SaveNotificationSetting(IsResultNotificationEnabled, NotificationSettingType.Result);
            }
        }

        private string _appDescription;
        public string AppDescription
        {
            get { return _appDescription; }
            set { Set(ref _appDescription, value); }
        }

        private string _scheduleCodeInputText;
        public string ScheduleCodeInputText {
            get { return _scheduleCodeInputText; }
            set { Set(ref _scheduleCodeInputText, value); }
        }

        public ICommand SwitchThemeCommand { get; private set; }
        public ICommand OnFeedbackEmailButtonClickCommand { get; private set; }
        public ICommand OnFeedbackAppButtonClickCommand { get; private set; }
        public ICommand OnLogoutButtonClickCommand { get; private set; }
        public ICommand OnScheduleCodeKeydownCommand { get; private set; }
        public ICommand OnScheduleCodeButtonClickCommand { get; private set; }
        public ICommand OnReviewStoreButtonClickCommand { get; private set; }
        private ProfileVM _user;
        public ProfileVM User {
            get { return _user; }
            set { Set(ref _user, value); }
        }
        private SettingsService Service { get; set; }

        public SettingsPageViewModel()
        {
            SearchBoxChanged = false;
            ScheduleCodeInputText = "";

            Service = Singleton<SettingsService>.Instance;

            User = new ProfileVM();
            SwitchThemeCommand = new RelayCommand(async () => {
                await ThemeSelectorService.SwitchThemeAsync();
            });
            OnFeedbackEmailButtonClickCommand = new RelayCommand(OnFeedbackEmailClick);
            OnLogoutButtonClickCommand = new RelayCommand(OnLogoutButtonClick);
            OnScheduleCodeKeydownCommand = new RelayCommand<KeyRoutedEventArgs>(OnScheduleCodeKeydown);
            OnScheduleCodeButtonClickCommand = new RelayCommand(OnScheduleCodeButtonClick);
            OnFeedbackAppButtonClickCommand = new RelayCommand(OnFeedbackAppClick);
            OnReviewStoreButtonClickCommand = new RelayCommand(OnReviewStoreClick);
        }

        public async void Initialize()
        {
            MainPageViewModel.SetPageTitle("Shell_SettingsPage".GetLocalized());
            IsLightThemeEnabled = ThemeSelectorService.IsLightThemeEnabled;
            AppDescription = Service.GetAppDescription();

            ScheduleCodeInputText = await Service.ReadScheduleCode();
            _isScheduleWithoutBlanksEnabled = await Service.ReadScheduleBlanks();

            _isAnnouncementNotificationEnabled = await Service.ReadKeyAsync(SettingsService.IsAnnouncementNotificationEnabledKey);
            _isDisruptionNotificationEnabled = await Service.ReadKeyAsync(SettingsService.IsDisruptionNotificationEnabledKey);
            _isResultNotificationEnabled = await Service.ReadKeyAsync(SettingsService.IsResultNotificationEnabledKey);
            
            User = await Singleton<ProfileService>.Instance.GetUserAsync();

            // Update UI
            OnPropertyChanged("IsScheduleWithoutBlanksEnabled");
            OnPropertyChanged("IsLightThemeEnabled");

            OnPropertyChanged("IsAnnouncementNotificationEnabled");
            OnPropertyChanged("IsDisruptionNotificationEnabled");
            OnPropertyChanged("IsResultNotificationEnabled");

        }
        
        
        private async void OnScheduleCodeButtonClick()
        {
            if (ScheduleCodeInputText != null)
            {
                if (SearchBoxChanged)
                {
                    SearchBoxChanged = false;
                    if (!string.IsNullOrEmpty(ScheduleCodeInputText))
                    {
                        await Service.SaveScheduleCode(ScheduleCodeInputText);

                        var dialog = new MessageDialog("DialogScheduleCodeSavedBody".GetLocalized(), "DialogScheduleCodeSavedHeader".GetLocalized());
                        await dialog.ShowAsync();
                    }
                    else
                    {
                        Service.RemoveKey(SettingsService.ScheduleCodeKey);

                        var dialog = new MessageDialog("DialogScheduleCodeSavedBody".GetLocalized(), "DialogScheduleCodeSavedHeader".GetLocalized());
                        await dialog.ShowAsync();
                    }
                }
                else if (string.IsNullOrWhiteSpace(ScheduleCodeInputText))
                {
                    Service.RemoveKey(SettingsService.ScheduleCodeKey);

                    var dialog = new MessageDialog("DialogScheduleCodeSavedBody".GetLocalized(), "DialogScheduleCodeSavedHeader".GetLocalized());
                    await dialog.ShowAsync();
                }

            }
        }
        private async void OnFeedbackEmailClick()
        {
            var contact = new Contact()
            {
                Name = "RLSmedia",
                LastName = "",
                MiddleName = "",
                FirstName = "",
                Nickname = "RLSmedia"
            };
            //contact.Websites.Add(new ContactWebsite() { Uri = new Uri("http://www.rlsmedia.nl/"), Description = "RLSmedia", RawValue = "http://www.rlsmedia.nl/" });
            contact.Emails.Add(new ContactEmail() { Address = "info@rlsmedia.nl", Description = "RLSmedia", Kind = ContactEmailKind.Work });

            // Get the device manufacturer, model name, OS details etc.
            string device_info = "";
            string newline = (DeviceTypeHelper.GetDeviceFormFactorType() == DeviceFormFactorType.Phone) ? "\r\n" : "<br />";

            if (ApiInformation.IsTypePresent("Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation"))
            {
                var clientDeviceInformation = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation();
                var deviceManufacturer = clientDeviceInformation.SystemManufacturer;
                var deviceModel = clientDeviceInformation.SystemProductName;
                var operatingSystem = clientDeviceInformation.OperatingSystem;
                var systemHardwareVersion = clientDeviceInformation.SystemHardwareVersion;
                var systemFirmwareVersion = clientDeviceInformation.SystemFirmwareVersion;
                device_info = "Fabrikant: " + deviceManufacturer.ToString() + newline + "Model: " + deviceModel.ToString() + newline + "OS: " + operatingSystem.ToString() + newline + "Hardware versie: " + systemHardwareVersion.ToString() + newline + "Firmware versie: " + systemFirmwareVersion.ToString();
            }
            string subject = "Feedback over Avans App";
            string body =   "Hallo, " + newline + newline +
                            "Type hier je bericht... " + newline + newline +
                            "Met vriendelijke groet, " + newline +
                            "Onbekend" + newline + newline + newline +
                            "Overige informatie: " + newline +
                            "Mijn App Versie: " + Package.Current.Id.Version.Revision.ToString() + newline +
                            "Mijn Build: " + Package.Current.Id.Version.Build.ToString() + newline +
                            "Architectuur: " + Package.Current.Id.Architecture + newline +
                            device_info;
            await Service.ComposeEmail(contact, subject, body, null);
        }

        private async void OnFeedbackAppClick()
        {
            if (Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.IsSupported())
            {
                var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
                await launcher.LaunchAsync();
            }
        }
        private async void OnReviewStoreClick()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9NBLGGH4S3FG"));
        }

        private async void OnLogoutButtonClick()
        {
            var dialog = new MessageDialog("LogoutMessageDialogContent".GetLocalized(), "LogoutMessageDialogHeader".GetLocalized());

            dialog.Commands.Add(new UICommand("AnswerOptionYes".GetLocalized()) { Id = 0 });
            dialog.Commands.Add(new UICommand("AnswerOptionNo".GetLocalized()) { Id = 1 });

            var response = await dialog.ShowAsync();

            if (response != null && response.Id.ToString() == "0")
            {
                Service.ClearAllSettings();
                NavigationService.NavigateToFrame(typeof(LoginPageViewModel).FullName, new Views.LoginPage());
            }
        }

        private void OnScheduleCodeKeydown(KeyRoutedEventArgs e)
        {
            if (e != null && e.KeyStatus.RepeatCount == 1)
            {
                if (e.Key == Windows.System.VirtualKey.Enter)
                    OnScheduleCodeButtonClick();
                else
                    SearchBoxChanged = true;
            }
        }
    }
}
