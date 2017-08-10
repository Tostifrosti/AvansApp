using System;
using AvansApp.Helpers;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;

namespace AvansApp.Services
{
    internal partial class ToastNotificationsService
    {
        public void Show(int count)
        {
            const string logo = @"ms-appx:///Assets/StoreLogo.png";
            
            // Create the toast content
            var content = new ToastContent()
            {
                // Documentation: https://developer.microsoft.com/en-us/windows/uwp-community-toolkit/api/microsoft_toolkit_uwp_notifications_toastcontent
                Launch = "ResultsNotification",
                Scenario = ToastScenario.Default,
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = (count > 1) ? 
                                    ("ToastNewResultHeader_0".GetLocalized() + " " + count + " " + "ToastNewResultHeader_1".GetLocalized())
                                    : ("ToastNewResultHeader".GetLocalized()),
                                HintMaxLines = 1
                            },
                            new AdaptiveText()
                            {
                                Text = "ToastNewResultText".GetLocalized(),
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = logo,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        }
                    }
                },
                DisplayTimestamp = DateTime.Now,
                Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        // Documentation: https://developer.microsoft.com/en-us/windows/uwp-community-toolkit/api/microsoft_toolkit_uwp_notifications_toastbutton
                        new ToastButton("ToastNotificationButton1".GetLocalized(), "ResultsNotification")
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
                Tag = "AvansResult", // Max length 16 characters.
                ExpirationTime = DateTime.Now.AddMonths(1),
                Group = "AvansResults"
            };

            // And show the toast
            ShowToastNotification(toast);
        }
    }
}
