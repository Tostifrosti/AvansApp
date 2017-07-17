using System;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.Storage;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.Email;
using Windows.Foundation.Metadata;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

using AvansApp.Models;
using AvansApp.Services;
using AvansApp.Helpers;

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
        private List<string> SettlementOptions { get; set; }

        private bool _isLightThemeEnabled;
        public bool IsLightThemeEnabled
        {
            get { return _isLightThemeEnabled; }
            set { Set(ref _isLightThemeEnabled, value); }
        }

        private string _appDescription;
        public string AppDescription
        {
            get { return _appDescription; }
            set { Set(ref _appDescription, value); }
        }

        private string scheduleCodeInputText;
        public string ScheduleCodeInputText {
            get { return scheduleCodeInputText; }
            set { Set(ref scheduleCodeInputText, value); }
        }
        private bool _isScheduleWithoutBlanksEnabled;
        public bool IsScheduleWithoutBlanksEnabled
        {
            get { return _isScheduleWithoutBlanksEnabled; }
            set { Set(ref _isScheduleWithoutBlanksEnabled, value); }
        }

        public ICommand SwitchThemeCommand { get; private set; }
        public ICommand OnFeedbackButtonClickCommand { get; private set; }
        public ICommand OnLogoutButtonClickCommand { get; private set; }
        public ICommand OnScheduleCodeKeydownCommand { get; private set; }
        public ICommand OnScheduleCodeButtonClickCommand { get; private set; }
        public ICommand OnScheduleBlanksToggleCommand { get; private set; }
        public ProfileVM User { get; set; }

        public SettingsPageViewModel()
        {
            SearchBoxChanged = false;
            SettlementOptions = new List<string> { "onderwijsboulevard", "hoofdgebouw" };

            User = new ProfileVM(); // TODO
            SwitchThemeCommand = new RelayCommand(async () => { await ThemeSelectorService.SwitchThemeAsync(); });
            OnFeedbackButtonClickCommand = new RelayCommand<ItemClickEventArgs>(OnFeedbackClick);
            OnLogoutButtonClickCommand = new RelayCommand<ItemClickEventArgs>(OnLogoutButtonClick);
            OnScheduleCodeKeydownCommand = new RelayCommand<KeyRoutedEventArgs>(OnScheduleCodeKeydown);
            OnScheduleCodeButtonClickCommand = new RelayCommand<ItemClickEventArgs>(OnScheduleCodeButtonClick);
            OnScheduleBlanksToggleCommand = new RelayCommand(OnScheduleBlanksToggle);

            if (OAuth.GetInstance().Client.CheckTokenExists("ScheduleCode"))
                ScheduleCodeInputText = OAuth.GetInstance().Client.GetTokenFromVault("ScheduleCode");

            IsScheduleWithoutBlanksEnabled = OAuth.GetInstance().Client.CheckTokenExists("ScheduleWithoutBlanks");

            /*Settlement.ItemsSource = SettlementOptions;
            if(main.CheckTokenExists("Settlement")) {
                string token = main.GetTokenFromVault("Settlement");
                Settlement.SelectedIndex = SettlementOptions.IndexOf(token);
            }*/
        }

        public void Initialize()
        {
            MainPageViewModel.SetPageTitle("Shell_SettingsPage".GetLocalized());
            IsLightThemeEnabled = ThemeSelectorService.IsLightThemeEnabled;
            AppDescription = GetAppDescription();
        }
        private string GetAppDescription()
        {
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{package.DisplayName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
        
        private async void OnScheduleCodeButtonClick(ItemClickEventArgs e)
        {
            if (ScheduleCodeInputText != null)
            {
                if (SearchBoxChanged)
                {
                    SearchBoxChanged = false;
                    if (!string.IsNullOrEmpty(ScheduleCodeInputText))
                    {
                        if (OAuth.GetInstance().Client.CheckTokenExists("ScheduleCode"))
                            OAuth.GetInstance().Client.RemoveFromVault("ScheduleCode");
                        OAuth.GetInstance().Client.AddToVault("ScheduleCode", "ScheduleCode", ScheduleCodeInputText);

                        var dialog = new MessageDialog("Je rooster code is succesvol opgeslagen!", "Rooster code");
                        await dialog.ShowAsync();
                    }
                    else
                    {
                        if (OAuth.GetInstance().Client.CheckTokenExists("ScheduleCode"))
                            OAuth.GetInstance().Client.RemoveFromVault("ScheduleCode");
                    }
                }

            }
        }
        /*private void Settlement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedItem = Settlement.SelectedValue.ToString().ToLower();
            if(main.CheckTokenExists("Settlement"))
            {
                main.RemoveFromVault("Settlement");
            }
            main.AddToVault("Settlement", "Settlement", selectedItem);
        }*/
        private async void OnFeedbackClick(ItemClickEventArgs e)
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

            string newline = (Helpers.DeviceTypeHelper.GetDeviceFormFactorType() == Helpers.DeviceFormFactorType.Phone) ? "\r\n" : "<br />";

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
            string body = "Hallo, " + newline + newline +
                            "Type hier je bericht... " + newline + newline +
                            "Met vriendelijke groet, " + newline +
                            "Onbekend" + newline + newline + newline +
                            "Overige informatie: " + newline +
                            "Mijn App Versie: " + Package.Current.Id.Version.Revision.ToString() + newline +
                            "Mijn Build: " + Package.Current.Id.Version.Build.ToString() + newline +
                            "Architectuur: " + Package.Current.Id.Architecture + newline +
                            device_info;
            await ComposeEmail(contact, subject, body, null);

        }
        private async void OnLogoutButtonClick(ItemClickEventArgs e)
        {
            var dialog = new MessageDialog("Weet je zeker dat je wilt uitloggen?", "Uitloggen");

            dialog.Commands.Add(new UICommand("Ja") { Id = 0 });
            dialog.Commands.Add(new UICommand("Nee") { Id = 1 });

            var response = await dialog.ShowAsync();

            if (response != null && response.Label == "Ja")
            {
                OAuth.GetInstance().Client.EmptyVault();
                NavigationService.NavigateToFrame(typeof(LoginPageViewModel).FullName, new Views.LoginPage());
            }

        }

        private async Task ComposeEmail(Contact recipient, string subject, string messageBody, StorageFile attachmentFile = null)
        {
            var emailMessage = new EmailMessage()
            {
                Body = messageBody,
                Subject = subject
            };
            if (attachmentFile != null)
            {
                var stream = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(attachmentFile);

                var attachment = new EmailAttachment(
                    attachmentFile.Name,
                    stream);

                emailMessage.Attachments.Add(attachment);
            }

            var email = recipient.Emails.FirstOrDefault();
            if (email != null)
            {
                var emailRecipient = new EmailRecipient(email.Address);
                emailMessage.To.Add(emailRecipient);
            }

            await EmailManager.ShowComposeNewEmailAsync(emailMessage);
        }

        private void OnScheduleCodeKeydown(KeyRoutedEventArgs e)
        {
            if (e != null && e.KeyStatus.RepeatCount == 1)
            {
                if (e.Key == Windows.System.VirtualKey.Enter)
                    OnScheduleCodeButtonClick(null);
                else
                    SearchBoxChanged = true;
            }
        }

        private void OnScheduleBlanksToggle()
        {
            IsScheduleWithoutBlanksEnabled = !IsScheduleWithoutBlanksEnabled;
            if (IsScheduleWithoutBlanksEnabled == false)
            {
                OAuth.GetInstance().Client.RemoveFromVault("ScheduleWithoutBlanks");
            }
            else
            {
                if (!OAuth.GetInstance().Client.CheckTokenExists("ScheduleWithoutBlanks"))
                    OAuth.GetInstance().Client.AddToVault("ScheduleWithoutBlanks", "ScheduleWithoutBlanks", "ScheduleWithoutBlanks");
            }
        }
    }
}
