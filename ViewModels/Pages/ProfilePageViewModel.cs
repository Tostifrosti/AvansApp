using System;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

using AvansApp.Helpers;

namespace AvansApp.ViewModels.Pages
{
    public class ProfilePageViewModel : ViewModelBase
    {
        private ProfileVM _myProfile;
        public ProfileVM MyProfile { get { return _myProfile; } private set { Set(ref _myProfile, value); } }
        private ImageSource _profileImage;
        public ImageSource ProfileImage { get { return _profileImage; } private set { Set(ref _profileImage, value); } }

        public ProfilePageViewModel()
        {
            MyProfile = new ProfileVM();
            ProfileImage = new BitmapImage(new Uri("ms-appx:///Assets/StoreLogo.png"));
        }

        public void Initialize(NavigationEventArgs e)
        {
            if (e != null)
            {
                MyProfile = e.Parameter as ProfileVM;
                if (MyProfile != null)
                {
                    ProfileImage = MyProfile.EmployeeImage.bitmap;
                }
            }
            MainPageViewModel.SetPageTitle("Shell_ProfilePage".GetLocalized());
        }
    }
}
