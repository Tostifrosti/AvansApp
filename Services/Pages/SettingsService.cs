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
    public class SettingsService
    {
        public const string ScheduleCodeKey = "ScheduleCode";
        public const string IsScheduleWithoutBlanksKey = "ScheduleWithoutBlanks";
        //public readonly List<string> SettlementOptions { get; set; }
        

        public SettingsService()
        {
            //SettlementOptions = new List<string> { "onderwijsboulevard", "hoofdgebouw" };
        }

        public async Task SaveScheduleCode(string scheduleCode)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync<string>(ScheduleCodeKey, scheduleCode);
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
        public async Task<bool> ReadScheduleBlanks()
        {
            return await ApplicationData.Current.LocalSettings.ReadAsync<bool>(IsScheduleWithoutBlanksKey);
        }
        public void RemoveKey(string key)
        {
            ApplicationData.Current.LocalSettings.RemoveKeyAsync(key);
        }
        public bool KeyExists(string key)
        {
            return ApplicationData.Current.LocalSettings.KeyExists(key);
        }

        public async Task ComposeEmail(Contact recipient, string subject, string messageBody, StorageFile attachmentFile = null)
        {
            var emailMessage = new EmailMessage()
            {
                Body = messageBody,
                Subject = subject
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
            RemoveKey(ScheduleCodeKey);
            RemoveKey(IsScheduleWithoutBlanksKey);

            // Delete Local storage
            Singleton<ResultService>.Instance.DeleteStorage();
            Singleton<AnnouncementService>.Instance.DeleteStorage();
            Singleton<DisruptionService>.Instance.DeleteStorage();
        }
    }
}
