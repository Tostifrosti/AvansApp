using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

using AvansApp.Services;

namespace AvansApp
{
    sealed partial class App : Application
    {
        private Lazy<ActivationService> _activationService;
        private ActivationService ActivationService { get { return _activationService.Value; } }


        public App()
        {
            InitializeComponent();
            _activationService = new Lazy<ActivationService>(CreateActivationService);
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (!e.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(e);
            }
        }
        
        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }
        protected override async void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }
        private ActivationService CreateActivationService()
        {
            /*if (Models.OAuth.GetInstance().Client.IsLoggedIn())
                return new ActivationService(this, typeof(ViewModels.Pages.SchedulePageViewModel), new Views.MainPage());
            else
                return new ActivationService(this, typeof(ViewModels.Pages.LoginPageViewModel), new Views.MainPage());*/

            return new ActivationService(this, typeof(ViewModels.Pages.SplashPageViewModel));
        }
    }
}
