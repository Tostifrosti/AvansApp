using System;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

using AvansApp.Helpers;
using AvansApp.Services.Pages;

namespace AvansApp.ViewModels.Pages
{
    public class ProfilePageViewModel : ViewModelBase
    {
        private ProfileVM _user;
        public ProfileVM User { get { return _user; } private set { Set(ref _user, value); } }
        private ImageSource _userImage;
        public ImageSource UserProfileImage { get { return _userImage; } private set { Set(ref _userImage, value); } }

        public ProfilePageViewModel()
        {
            User = new ProfileVM();
            UserProfileImage = new BitmapImage(new Uri("ms-appx:///Assets/StoreLogo.png"));
        }

        public async void Initialize(NavigationEventArgs e)
        {
            MainPageViewModel.SetPageTitle("Shell_ProfilePage".GetLocalized());

            User = await Singleton<ProfileService>.Instance.GetUserAsync();
            UserProfileImage = User.EmployeeImage?.bitmap;
        }
    }
}
