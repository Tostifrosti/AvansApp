using AvansApp.Helpers;
using AvansApp.Services;
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.ApplicationModel.Contacts;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;

namespace AvansApp.ViewModels.Pages
{
    public class EmployeeSinglePageViewModel : ViewModelBase
    {
        public NavigationService NavigationService
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<NavigationService>();
            }
        }

        private EmployeeVM _item;
        public EmployeeVM Item
        {
            get { return _item; }
            set { Set(ref _item, value); }
        }
        public ImageSource ProfileImage { get; set; }

        //public ICommand ContactClickCommand { get; private set; }
        public ICommand PhoneClickCommand { get; private set; }
        public ICommand EmailClickCommand { get; private set; }
        public ICommand ViewScheduleClickCommand { get; private set; }

        public EmployeeSinglePageViewModel()
        {
            //ContactClickCommand = new RelayCommand(OnContactClickAsync);
            PhoneClickCommand = new RelayCommand(OnPhoneClick);
            EmailClickCommand = new RelayCommand(OnEmailClick);
            ViewScheduleClickCommand = new RelayCommand(OnViewScheduleClick);
        }
        public void Initialize()
        {
            MainPageViewModel.SetPageTitle(Item.FullName);
        }

        public async void SetEmployeeImage()
        {
            try
            {
                Item.ProfilePicture.bitmap = await Item.ProfilePicture.Base64StringToBitmap(Item.ProfilePicture.image);
                ProfileImage = Item.ProfilePicture.bitmap;
            }
            catch (Exception Error)
            {
                System.Diagnostics.Debug.WriteLine(Error.Message);
            }
        }

        private async void OnContactClickAsync() // Bug
        {
            if (Item == null)
                return;

            // Create contact
            Contact contact = new Contact()
            {
                FirstName = Item.FirstName,
                LastName = Item.Surname
            };

            if (Item.Email != null)
            {
                contact.Emails.Add(new ContactEmail()
                {
                    Address = Item.Email,
                    Kind = ContactEmailKind.Work
                });
            }
            if (Item.PrivateNumber != null)
            {
                contact.Phones.Add(new ContactPhone()
                {
                    Number = Item.PrivateNumber,
                    Kind = ContactPhoneKind.Work
                });
            }

            if (Item.ProfilePicture != null && Item.ProfilePicture.bitmap != null)
            {
                // read stream
                var bytes = Convert.FromBase64String(Item.ProfilePicture.image);
                IRandomAccessStream random = bytes.AsBuffer()?.AsStream()?.AsRandomAccessStream();
                
                if (random != null)
                {
                    contact.SourceDisplayPicture = RandomAccessStreamReference.CreateFromStream(random);
                }
            }

            // Serialize & add to uri
            string contactStr = await Json.StringifyAsync(contact);
            string contactUri = @"ms-people:viewcontact?Contact=" + contactStr;


            // The URI to launch
            var uri = new Uri(contactUri);

            // Options
            var options = new Windows.System.LauncherOptions();
            options.DisplayApplicationPicker = true;

            // Launch the URI
            var success = await Windows.System.Launcher.LaunchUriAsync(uri, options);
        }
        private async void OnPhoneClick()
        {
            if (Item == null || Item.PrivateNumber == null || !IsAllDigits(Item.PrivateNumber))
                return;

            string contactStr = @"ms-people:viewcontact?ContactName=" + Item.VolledigeNaam + "&PhoneNumber=" + Item.PrivateNumber;
            Uri contactUri = new Uri(contactStr);

            // Options
            var options = new Windows.System.LauncherOptions();
            options.DisplayApplicationPicker = true;

            // The URI to launch
            await Windows.System.Launcher.LaunchUriAsync(contactUri, options);
        }

        private async void OnEmailClick()
        {
            if (Item == null || Item.Email == null)
                return;

            string contactStr = @"ms-people:viewcontact?ContactName=" + Item.VolledigeNaam + "&Email=" + Item.Email;
            Uri contactUri = new Uri(contactStr);

            // Options
            var options = new Windows.System.LauncherOptions();
            options.DisplayApplicationPicker = true;

            // The URI to launch
            await Windows.System.Launcher.LaunchUriAsync(contactUri, options);
        }

        private void OnViewScheduleClick()
        {
            if (Item == null)
                return;

            NavigationService.Navigate(typeof(EmployeeScheduleSinglePageViewModel).FullName, Item);
        }

        private bool IsAllDigits(string s)
        {
            if (s == null)
                return false;

            foreach (char c in s)
            {
                if (!(c.Equals('+') || c.Equals('-') || char.IsDigit(c)))
                    return false;
            }
            return true;
        }
    }
}
