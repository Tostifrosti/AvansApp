using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Notifications;

using AvansApp.Activation;
using AvansApp.ViewModels.Pages;
using AvansApp.Models;

namespace AvansApp.Services
{
    internal partial class ToastNotificationsService : ActivationHandler<ToastNotificationActivatedEventArgs>
    {
        public NavigationService NavigationService
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<NavigationService>();
            }
        }

        public void ShowToastNotification(ToastNotification toastNotification)
        {
            ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
        }

        protected override async Task HandleInternalAsync(ToastNotificationActivatedEventArgs args)
        {
            // Documentation: https://blogs.msdn.microsoft.com/tiles_and_toasts/2015/07/08/quickstart-sending-a-local-toast-notification-and-handling-activations-from-it-windows-10/

            if (OAuth.GetInstance().Client.IsLoggedIn())
            {
                if (args.Argument.Contains("AnnouncementsNotification"))
                {
                    // Navigate to Announcement Page
                    NavigationService.NavigateToPage(typeof(AnnouncementPageViewModel).FullName, new Views.MainPage());
                }
                else if (args.Argument.Contains("ResultsNotification"))
                {
                    // Navigate to Results Page
                    NavigationService.NavigateToPage(typeof(ResultPageViewModel).FullName, new Views.MainPage());
                }
                else if (args.Argument.Contains("DisruptionsNotification"))
                {
                    // Navigate to Disruptions Page
                    NavigationService.NavigateToPage(typeof(DisruptionPageViewModel).FullName, new Views.MainPage());
                }
                else if (args.Argument.Contains("ExamsNotification"))
                {
                    // Navigate to Exams Page
                    NavigationService.NavigateToPage(typeof(ExamPageViewModel).FullName, new Views.MainPage());
                }
            }
            else {
                // Navigate to Splashscreen
                NavigationService.NavigateToPage(typeof(SplashPageViewModel).FullName, new SplashPage());
            }
            await Task.CompletedTask;
        }
    }
}
