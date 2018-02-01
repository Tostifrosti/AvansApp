using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.ApplicationModel.Background;

using AvansApp.Helpers;
using AvansApp.Services;
using AvansApp.Services.Pages;
using AvansApp.Models.ServerModels;

namespace AvansApp.BackgroundTasks
{
    public sealed class DisruptionsBackgroundTask : BackgroundTask
    {
        private const string Name = "DisruptionBackgroundTask";
        private BackgroundTaskDeferral _deferral;
        private SettingsService Settings = Singleton<SettingsService>.Instance;
        private volatile bool IsCanceled = false;

        public override async void Register()
        {
            var taskName = Name;
            IsCanceled = false;

            bool IsNotificationEnabled = await Settings.ReadKeyAsync(SettingsService.IsDisruptionNotificationEnabledKey);

            if (!IsNotificationEnabled)
            {
                BackgroundTaskRegistration task = BackgroundTaskService.GetBackgroundTasksRegistration<DisruptionsBackgroundTask>();
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

                builder.SetTrigger(new TimeTrigger(30, false)); // 30 minutes cycle, Note: timer cannot be less than 15 mins
                //builder.AddCondition(new SystemCondition(SystemConditionType.FreeNetworkAvailable));
                builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
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
                bool IsNotificationEnabled = await Settings.ReadKeyAsync(SettingsService.IsDisruptionNotificationEnabledKey);

                if (!IsNotificationEnabled || IsCanceled)
                {
                    taskInstance.Task.Unregister(true);
                    return;
                }

                DisruptionService service = Singleton<DisruptionService>.Instance;
                List<DisruptionItem> items = await service.Request();

                int countNewItems = await service.CompareNewDisruptionsAsync(items);

                if (countNewItems > 0)
                {
                    Singleton<ToastNotificationsService>.Instance.Show(countNewItems, ToastNotificationsService.NotificationType.Disruptions);
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
