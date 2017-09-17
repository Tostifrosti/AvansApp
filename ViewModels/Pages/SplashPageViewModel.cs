using System;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using Windows.UI;
using Windows.Foundation.Metadata;

using AvansApp.Models;
using AvansApp.Helpers;
using AvansApp.Services;
using AvansApp.Models.ServerModels;

namespace AvansApp.ViewModels.Pages
{
    public class SplashPageViewModel: ViewModelBase
    {
        public NavigationService NavigationService
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<NavigationService>();
            }
        }
        private ProfileVM User { get; set; }

        private string _progressionText;
        public string ProgressionText { get { return _progressionText; } private set { Set(ref _progressionText, value); } }
        private bool _isProgressionRingActive;
        public bool IsProgressionRingActive { get { return _isProgressionRingActive; } private set { Set(ref _isProgressionRingActive, value); } }
        private bool _isTryAgainButtonVisible;
        public bool IsTryAgainButtonVisible { get { return _isTryAgainButtonVisible; } private set { Set(ref _isTryAgainButtonVisible, value); } }
        public ICommand OnTryAgainButtonClickCommand { get; private set; }
        public ICommand OnDisruptionsButtonClickCommand { get; private set; }

        public SplashPageViewModel()
        {
            OnTryAgainButtonClickCommand = new RelayCommand(() => { GetUserInfoAsync(); });
            OnDisruptionsButtonClickCommand = new RelayCommand(() => {
                NavigationService.NavigateToFrame(typeof(DisruptionPageViewModel).FullName, new Views.DisruptionsPage());
            });
        }

        public void Initialize()
        {
            ProgressionText = "";
            IsProgressionRingActive = true;
            IsTryAgainButtonVisible = false;

            if (OAuth.GetInstance().Client.IsLoggedIn())
            {
                // Welcome!
                GetUserInfoAsync();
            }
            else
            {
                // Oh oh! Looks like you need to login first!
                NavigationService.NavigateToFrame(typeof(LoginPageViewModel).FullName, new Views.LoginPage());
                //NavigationService.NavigateToFrame(typeof(LoginPageViewModel).FullName, new Windows.UI.Xaml.Controls.Frame());
                //NavigationService.Navigate(typeof(LoginPageViewModel).FullName);
            }
        }
        

        private async void GetUserInfoAsync()
        {
            ProgressionText = "SplashPageProgressionText1".GetLocalized();
            IsProgressionRingActive = true;
            IsTryAgainButtonVisible = false;

            User = new ProfileVM {
                Name = "John Doe",
                Title = "",
                Emailadres = "johndoe@email.nl",
                ProfilePicture = "/Assets/StoreLogo.png"
            };

            Student[] s = await OAuth.GetInstance().RequestJSON<Student[]>("https://publicapi.avans.nl/oauth/studentnummer/", new List<string>(), Models.Enums.HttpMethod.GET);

            if (s != null && s[0] != null)
            {
                ProgressionText = "SplashPageProgressionText2".GetLocalized();
                Student student = s[0];
                People people = await OAuth.GetInstance().RequestJSON<People>("https://publicapi.avans.nl/oauth/people/" + student.inlognaam, new List<string>(), Models.Enums.HttpMethod.GET);
                if (people != null)
                {
                    User = new ProfileVM {
                        Firstname = people.name.givenName,
                        Surname = people.name.familyName,
                        Name = people.name.formatted,
                        Login = student.inlognaam,
                        Studentnummer = student.studentnummer,
                        Emailadres = people.emails.FirstOrDefault(),
                        Title = people.title,
                        ProfilePicture = "/Assets/StoreLogo.png",
                        EmployeeImage = new EmployeeImage() { path = "../Assets/StoreLogo.png" }
                    };
            

                    ProgressionText = "SplashPageProgressionText3".GetLocalized() + " " + User.Fullname + "!";
                    IsProgressionRingActive = false;
                    IsTryAgainButtonVisible = false;
                }
                
                NavigationService.NavigateToFrame(typeof(SchedulePageViewModel).FullName, new Views.MainPage());
            }
            else
            {
                // We cannot connect to the Server!
                ProgressionText = "SplashPageProgressionTextError".GetLocalized();
                IsProgressionRingActive = false;
                IsTryAgainButtonVisible = true;
            }
        }

        private async void ShowStatusBar(Color background, Color foreground)
        {
            // Turn on SystemTray for mobile
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                // Added Reference: Windows Desktop Extensions for the UWP
                // Added Reference: Windows Mobile Extensions for the UWP
                var statusbar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                await statusbar.ShowAsync();
                statusbar.BackgroundOpacity = 1;
                statusbar.BackgroundColor = background;
                statusbar.ForegroundColor = foreground;
            }
        }
        
    }
}
