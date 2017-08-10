﻿using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.ApplicationModel.Background;

using AvansApp.Helpers;
using AvansApp.Services;
using AvansApp.Services.Pages;
using AvansApp.Models.ServerModels;

namespace AvansApp.BackgroundTasks
{
    public sealed class ResultsBackgroundTask : BackgroundTask
    {
        public static string Message;

        //private volatile bool _cancelRequested = false;
        //private IBackgroundTaskInstance _taskInstance;
        private BackgroundTaskDeferral _deferral;

        public override void Register()
        {
            var taskName = GetType().Name;

            if (!BackgroundTaskRegistration.AllTasks.Any(t => t.Value.Name == taskName))
            {
                var builder = new BackgroundTaskBuilder()
                {
                    Name = taskName
                };
                
                builder.SetTrigger(new TimeTrigger(30, false)); // 30 minutes cycle, debug: 1 minute
                builder.AddCondition(new SystemCondition(SystemConditionType.FreeNetworkAvailable));

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
                

                ResultService service = Singleton<ResultService>.Instance;
                List<Result> results = await service.RequestResults();

                int countNewResults = await service.CompareNewResultsAsync(results);

                if (countNewResults > 0)
                {
                    Singleton<ToastNotificationsService>.Instance.Show(countNewResults);
                }

                _deferral.Complete();
            });
        }

        public override void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            // TODO UWPTemplates: Insert code to handle the cancelation request here. 
            // Documentation: https://docs.microsoft.com/windows/uwp/launch-resume/handle-a-cancelled-background-task
        }
    }
}
