using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.Email;

using AvansApp.Helpers;

namespace AvansApp.Services.Pages
{
    public enum NotificationSettingType
    {
        Announcement,
        Disruption,
        Result
    }

    public class SettingsService
    {
        public const string ScheduleCodeKey = "ScheduleCode";
        public const string IsScheduleWithoutBlanksKey = "ScheduleWithoutBlanks";
        public const string IsAnnouncementNotificationEnabledKey = "AnnouncementNotificationKey";
        public const string IsDisruptionNotificationEnabledKey = "DisruptionNotificationKey";
        public const string IsResultNotificationEnabledKey = "ResultNotificationKey";
        //public readonly List<string> SettlementOptions { get; set; }


        public SettingsService()
        {
            //SettlementOptions = new List<string> { "onderwijsboulevard", "hoofdgebouw" };
        }

        public async Task SaveScheduleCode(string scheduleCode)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync<string>(ScheduleCodeKey, scheduleCode);
            Singleton<ScheduleService>.Instance.EmptySchedule();
        }
        public async Task<string> ReadScheduleCode()
        {
            return await ApplicationData.Current.LocalSettings.ReadAsync<string>(ScheduleCodeKey);
        }

        public string GetAppDescription()
        {
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{package.DisplayName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        public async Task SaveScheduleBlanks(bool isWithoutBlanks)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync(IsScheduleWithoutBlanksKey, isWithoutBlanks);
        }
        public async Task SaveNotificationSetting(bool isEnabled, NotificationSettingType type)
        {
            switch(type)
            {
                case NotificationSettingType.Announcement:
                    await ApplicationData.Current.LocalSettings.SaveAsync(IsAnnouncementNotificationEnabledKey, isEnabled);
                    break;
                case NotificationSettingType.Disruption:
                    await ApplicationData.Current.LocalSettings.SaveAsync(IsDisruptionNotificationEnabledKey, isEnabled);
                    break;
                case NotificationSettingType.Result:
                    await ApplicationData.Current.LocalSettings.SaveAsync(IsResultNotificationEnabledKey, isEnabled);
                    break;
            }
            Singleton<BackgroundTaskService>.Instance.RegisterBackgroundTasks();
        }
        public async Task<bool> ReadScheduleBlanks()
        {
            return await ApplicationData.Current.LocalSettings.ReadAsync<bool>(IsScheduleWithoutBlanksKey);
        }
        public void RemoveKey(string key)
        {
            ApplicationData.Current.LocalSettings.RemoveKey(key);
        }
        public bool KeyExists(string key)
        {
            return ApplicationData.Current.LocalSettings.KeyExists(key);
        }
        public async Task<bool> ReadKeyAsync(string key)
        {
            return await ApplicationData.Current.LocalSettings.ReadAsync<bool>(key);
        }

        public async Task ComposeEmail(Contact recipient, string subject, string messageBody, StorageFile attachmentFile = null)
        {
            var emailMessage = new EmailMessage()
            {
                Subject = subject,
                Body = messageBody
            };
            if (attachmentFile != null)
            {
                var stream = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(attachmentFile);

                var attachment = new EmailAttachment(
                    attachmentFile.Name,
                    stream);

                emailMessage.Attachments.Add(attachment);
            }

            var email = recipient.Emails.FirstOrDefault();
            if (email != null)
            {
                var emailRecipient = new EmailRecipient(email.Address);
                emailMessage.To.Add(emailRecipient);
            }

            await EmailManager.ShowComposeNewEmailAsync(emailMessage);
        }

        public void ClearAllSettings()
        {
            // Remove Setting Keys
            RemoveKey(ScheduleCodeKey);
            RemoveKey(IsScheduleWithoutBlanksKey);
            RemoveKey(IsAnnouncementNotificationEnabledKey);
            RemoveKey(IsDisruptionNotificationEnabledKey);
            RemoveKey(IsResultNotificationEnabledKey);
            RemoveKey(ThemeSelectorService.SettingsKey);

            // Delete Local storage
            Singleton<ResultService>.Instance.DeleteStorage();
            Singleton<AnnouncementService>.Instance.DeleteStorage();
            Singleton<DisruptionService>.Instance.DeleteStorage();

            // Empty vault
            Models.OAuth.GetInstance().Client.EmptyVault();

            // Unregister all background tasks
            BackgroundTaskService.CancelAllBackgroundTasks();

            Singleton<SettingsService>.ClearInstances();
        }
    }
}
