using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Data.Xml.Dom;

using AvansApp.Models;
using AvansApp.Helpers;
using AvansApp.ViewModels;
using AvansApp.Models.ServerModels;

namespace AvansApp.Services.Pages
{
    public class AnnouncementService
    {
        private const string base_url = "https://publicapi.avans.nl/oauth";
        private static List<AnnouncementVM> Items { get; set; }
        private const string StorageKey = "AnnouncementStorage";
        private DateTime refreshTime;

        public AnnouncementService()
        {
            refreshTime = DateTime.Now;
            Items = null;
        }

        public async Task<List<AnnouncementVM>> GetAnnouncements()
        {
            if (Items == null || refreshTime < DateTime.Now.AddMinutes(-1)) {
                refreshTime = DateTime.Now;

                Items = new List<AnnouncementVM>();

                List<Announcement> storage = await GetFromStorage();
                List<Announcement> newAnnouncements = await Request();

                if (await CompareNewAnnouncementsAsync(newAnnouncements) <= 0)
                {
                    storage = await GetFromStorage();

                    foreach (Announcement a in storage)
                        Items.Add(new AnnouncementVM(a));

                    return Items;
                }
                else
                {
                    // New announcements!
                    storage = await GetFromStorage();

                    foreach (Announcement a in storage)
                        Items.Add(new AnnouncementVM(a));
                }
            }
            return Items;
        }

        public async Task<List<Announcement>> Request()
        {
            XmlDocument doc = await OAuth.GetInstance().RequestXML(base_url + "/bb/ann/", new List<string>(), Models.Enums.HttpMethod.GET);
            List<Announcement> result = new List<Announcement>();

            if (doc != null && doc.DocumentElement != null && doc.DocumentElement.ChildNodes.Count > 1)
            {
                var xml = doc.DocumentElement;

                foreach (var announcements in xml.ChildNodes)
                {
                    if (announcements.ChildNodes.Count > 0)
                    {
                        Announcement item = new Announcement();
                        foreach (var node in announcements.ChildNodes)
                        {
                            if (node.LocalName != null)
                            {
                                switch (node.LocalName.ToString())
                                {
                                    case "title":
                                        string title = node.InnerText.ToString().Trim();
                                        item.title = title;
                                        break;
                                    case "course":
                                        item.course = node.InnerText.ToString();
                                        break;
                                    case "link":
                                        item.link = node.InnerText.ToString();
                                        break;
                                    case "datetime":
                                        DateTime date;
                                        if (DateTime.TryParse(node.InnerText.ToString(), out date))
                                            item.datetime = date;
                                        else
                                            item.datetime = new DateTime(); //node.InnerText.ToString()
                                        break;
                                    case "message":
                                        item.message = node.InnerText.ToString();
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        public async Task<int> CompareNewAnnouncementsAsync(List<Announcement> newItems)
        {
            if (newItems == null || newItems.Count <= 0)
                return 0;

            List<Announcement> storage = await GetFromStorage();
            int foundNewItems = 0;

            if (storage == null || storage.Count <= 0)
            {
                storage = new List<Announcement>();
                
                // Storage is empty
                foreach (Announcement item in newItems)
                {
                    storage.Add(item);
                    foundNewItems++;
                }
                // No need to sort
                await SaveToStorage(storage);

                return foundNewItems;
            }
            else
            {
                // Delete old items
                for (int i = 0; i < storage.Count; i++)
                {
                    if (storage[i].datetime <= DateTime.Now.AddMonths(-3))
                    {
                        storage.RemoveAt(i);
                        i--;
                    }
                }
                int temp = -1;

                // Comprare & add/update new items
                foreach (Announcement item in newItems)
                {
                    temp = -1;
                    for (int i = 0; i < storage.Count; i++)
                    {
                        if (Compare(item, storage[i]))
                        {
                            temp = i;
                            break;
                        }
                    }

                    if (temp < 0)
                    {
                        foundNewItems++;
                        storage.Add(item);
                    }
                    else
                    {
                        storage[temp] = item; // Update announcement
                    }
                }

                // No need to sort
                await SaveToStorage(storage);
            }

            return foundNewItems;
        }
        private async Task<List<Announcement>> GetFromStorage()
        {
            if (ApplicationData.Current.LocalFolder.FileExists(StorageKey))
            {
                return await ApplicationData.Current.LocalFolder.ReadAsync<List<Announcement>>(StorageKey);
            }
            else
            {
                // First time 
                return new List<Announcement>();
            }
        }
        private async Task SaveToStorage(List<Announcement> items)
        {
            if (items == null || items.Count <= 0)
                return;

            await ApplicationData.Current.LocalFolder.SaveAsync(StorageKey, items);
        }
        public bool Compare(Announcement a, Announcement b)
        {
            if (a == null || b == null)
                return false;

            if (a.course == b.course &&
                a.link == b.link && 
                a.title == b.title)
            {
                return true;
            }
            return false;
        }
        public bool Compare(AnnouncementVM a, AnnouncementVM b)
        {
            if (a == null || b == null)
                return false;

            if (a.Course == b.Course &&
                a.Link == b.Link &&
                a.Title == b.Title)
            {
                return true;
            }
            return false;
        }
        public void DeleteStorage()
        {
            ApplicationData.Current.LocalFolder.DeleteFile(StorageKey);
        }
    }
}
