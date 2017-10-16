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

        private int _amountPassingGrades;
        public int AmountPassingGrades {
            get { return _amountPassingGrades; }
            set { Set(ref _amountPassingGrades, value); }
        }
        private int _amountFailingGrades;
        public int AmountFailingGrades
        {
            get { return _amountFailingGrades; }
            set { Set(ref _amountFailingGrades, value); }
        }
        private double _averageGrade;
        public double AverageGrade
        {
            get { return _averageGrade; }
            set { Set(ref _averageGrade, value); }
        }
        private double _totalEC;
        public double TotalEC
        {
            get { return _totalEC; }
            set { Set(ref _totalEC, value); }
        }

        public ProfileService Service { get; private set; }

        public ProfilePageViewModel()
        {
            User = new ProfileVM();
            UserProfileImage = new BitmapImage(new Uri("ms-appx:///Assets/StoreLogo.png"));
            Service = Singleton<ProfileService>.Instance;
        }

        public async void Initialize(NavigationEventArgs e)
        {
            MainPageViewModel.SetPageTitle("Shell_ProfilePage".GetLocalized());

            User = await Service.GetUserAsync();
            UserProfileImage = User.EmployeeImage?.bitmap;

            AmountPassingGrades = await Service.GetAmountPassingGrades();
            AmountFailingGrades = await Service.GetAmountFailingGrades();
            AverageGrade = await Service.GetAverageGrade();
            TotalEC = await Service.GetTotalEC();
        }
    }
}
