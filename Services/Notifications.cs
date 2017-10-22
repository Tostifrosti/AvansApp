using System;
using AvansApp.Helpers;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;

namespace AvansApp.Services
{
    internal partial class ToastNotificationsService
    {
        public enum NotificationType
        {
            Results,
            Disruptions,
            Exams,
            Announcements
        }

        public void Show(int count, NotificationType type)
        {
            ToastNotification toast = null;

            switch (type)
            {
                case NotificationType.Announcements:
                    toast = ShowAnnouncementNotification(count);
                    break;
                case NotificationType.Results:
                    toast = ShowResultNotification(count);
                    break;
                case NotificationType.Disruptions:
                    toast = ShowDisruptionNotification(count);
                    break;
                default:
                    break;
            }


            // And show the toast
            if (toast != null)
                ShowToastNotification(toast);
        }

        private ToastNotification ShowResultNotification(int count)
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
                DisplayTimestamp = DateTime.Now
                /*Actions = new ToastActionsCustom()
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
                }*/
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
            return toast;
        }

        private ToastNotification ShowDisruptionNotification(int count)
        {
            const string logo = @"ms-appx:///Assets/StoreLogo.png";

            // Create the toast content
            var content = new ToastContent()
            {
                // Documentation: https://developer.microsoft.com/en-us/windows/uwp-community-toolkit/api/microsoft_toolkit_uwp_notifications_toastcontent
                Launch = "DisruptionsNotification",
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
                                    ("ToastNewDisruptionHeader_0".GetLocalized() + " " + count + " " + "ToastNewDisruptionHeader_1".GetLocalized())
                                    : ("ToastNewDisruptionHeader".GetLocalized()),
                                HintMaxLines = 1
                            },
                            new AdaptiveText()
                            {
                                Text = "ToastNewDisruptionText".GetLocalized(),
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = logo,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        }
                    }
                },
                DisplayTimestamp = DateTime.Now
                /*Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        // Documentation: https://developer.microsoft.com/en-us/windows/uwp-community-toolkit/api/microsoft_toolkit_uwp_notifications_toastbutton
                        new ToastButton("ToastNotificationButton1".GetLocalized(), "DisruptionsNotification")
                        {
                            ActivationType = ToastActivationType.Foreground
                        },

                        //new ToastButtonDismiss("Cancel")
                    }
                }*/
            };

            // Create the toast
            var toast = new ToastNotification(content.GetXml())
            {
                // Gets or sets the unique identifier of this notification within the notification Group. 
                // Documentation: https://docs.microsoft.com/uwp/api/windows.ui.notifications.toastnotification
                Tag = "AvansDisruption", // Max length 16 characters.
                ExpirationTime = DateTime.Now.AddMonths(1),
                Group = "AvansDisruptions"
            };
            return toast;
        }

        private ToastNotification ShowAnnouncementNotification(int count)
        {
            const string logo = @"ms-appx:///Assets/StoreLogo.png";

            // Create the toast content
            var content = new ToastContent()
            {
                // Documentation: https://developer.microsoft.com/en-us/windows/uwp-community-toolkit/api/microsoft_toolkit_uwp_notifications_toastcontent
                Launch = "AnnouncementsNotification",
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
                                    ("ToastNewAnnouncementHeader_0".GetLocalized() + " " + count + " " + "ToastNewAnnouncementHeader_1".GetLocalized())
                                    : ("ToastNewAnnouncementHeader".GetLocalized()),
                                HintMaxLines = 1
                            },
                            new AdaptiveText()
                            {
                                Text = "ToastNewAnnouncementText".GetLocalized(),
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = logo,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        }
                    }
                },
                DisplayTimestamp = DateTime.Now
            };

            // Create the toast
            var toast = new ToastNotification(content.GetXml())
            {
                // Gets or sets the unique identifier of this notification within the notification Group. 
                // Documentation: https://docs.microsoft.com/uwp/api/windows.ui.notifications.toastnotification
                Tag = "AvansAnnmnt", // Max length 16 characters.
                ExpirationTime = DateTime.Now.AddMonths(1),
                Group = "AvansAnnGroup"
            };
            return toast;
        }
    }
}
