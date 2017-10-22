using AvansApp.Helpers;
using AvansApp.Models.ServerModels;
using AvansApp.Services;
using AvansApp.Services.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace AvansApp.BackgroundTasks
{
    public class AnnouncementsBackgroundTask : BackgroundTask
    {
        private const string Name = "AnnouncementBackgroundTask";
        private BackgroundTaskDeferral _deferral;
        private SettingsService Settings = Singleton<SettingsService>.Instance;
        private bool IsCanceled { get; set; }

        public override async void Register()
        {
            var taskName = Name;
            IsCanceled = false;

            bool IsNotificationEnabled = await Settings.ReadKeyAsync(SettingsService.IsAnnouncementNotificationEnabledKey);
            BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();

            if (!IsNotificationEnabled || status.HasFlag(BackgroundAccessStatus.DeniedBySystemPolicy | BackgroundAccessStatus.DeniedByUser | BackgroundAccessStatus.Unspecified))
            {
                BackgroundTaskRegistration task = BackgroundTaskService.GetBackgroundTasksRegistration<AnnouncementsBackgroundTask>();
                if (task != null)
                {
                    task.Unregister(true);
                }
                return;
            }

            if (!BackgroundTaskRegistration.AllTasks.Any(t => t.Value.Name == taskName))
            {
                var builder = new BackgroundTaskBuilder()
                {
                    Name = taskName
                };

                builder.SetTrigger(new TimeTrigger(30, false)); // 30 minutes cycle
                //builder.AddCondition(new SystemCondition(SystemConditionType.FreeNetworkAvailable));
                builder.AddCondition(new SystemCondition(SystemConditionType.UserNotPresent));

                builder.Register();
            }
        }

        public override Task RunAsyncInternal(IBackgroundTaskInstance taskInstance)
        {
            if (taskInstance == null)
                return null;

            _deferral = taskInstance.GetDeferral();

            return Task.Run(async () =>
            {
                // Documentation: 
                //      * General: https://docs.microsoft.com/en-us/windows/uwp/launch-resume/support-your-app-with-background-tasks
                //      * Debug: https://docs.microsoft.com/en-us/windows/uwp/launch-resume/debug-a-background-task 
                //      * Monitoring: https://docs.microsoft.com/windows/uwp/launch-resume/monitor-background-task-progress-and-completion

                bool IsNotificationEnabled = await Settings.ReadKeyAsync(SettingsService.IsAnnouncementNotificationEnabledKey);

                if (!IsNotificationEnabled || IsCanceled)
                {
                    taskInstance.Task.Unregister(true);
                    return;
                }

                AnnouncementService service = Singleton<AnnouncementService>.Instance;
                List<Announcement> results = await service.Request();

                int countNewAnnouncements = await service.CompareNewAnnouncementsAsync(results);

                if (countNewAnnouncements > 0)
                {
                    Singleton<ToastNotificationsService>.Instance.Show(countNewAnnouncements, ToastNotificationsService.NotificationType.Announcements);
                }

                _deferral.Complete();
            });
        }

        public override void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            IsCanceled = true;
        }

        public override string GetName()
        {
            return Name;
        }
    }
}
