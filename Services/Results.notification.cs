using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;

namespace AvansApp.Services
{
    internal partial class ToastNotificationsService
    {
        public void Show()
        {
            // Create the toast content
            var content = new ToastContent()
            {
                // Documentation: https://developer.microsoft.com/en-us/windows/uwp-community-toolkit/api/microsoft_toolkit_uwp_notifications_toastcontent
                Launch = "ToastContentActivationParams",

                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = "Je hebt een nieuw resultaat!"
                            },

                            new AdaptiveText()
                            {
                                 Text = @"Er staat een nieuw resultaat voor je klaar!"
                            }
                        }
                    }
                },

                Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        // Documentation: https://developer.microsoft.com/en-us/windows/uwp-community-toolkit/api/microsoft_toolkit_uwp_notifications_toastbutton
                        new ToastButton("Bekijken", "ToastButtonActivationArguments")
                        {
                            ActivationType = ToastActivationType.Foreground
                        },

                        //new ToastButtonDismiss("Cancel")
                    }
                }
            };

            // Create the toast
            var toast = new ToastNotification(content.GetXml())
            {
                // Gets or sets the unique identifier of this notification within the notification Group. 
                // Documentation: https://docs.microsoft.com/uwp/api/windows.ui.notifications.toastnotification
                Tag = "AvansResult" // Max length 16 characters.
            };

            // And show the toast
            ShowToastNotification(toast);
        }
    }
}
