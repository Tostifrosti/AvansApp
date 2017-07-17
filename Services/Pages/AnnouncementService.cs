using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;

using AvansApp.Models;
using AvansApp.ViewModels;
using AvansApp.Models.ServerModels;

namespace AvansApp.Services.Pages
{
    public class AnnouncementService
    {
        private const string base_url = "https://publicapi.avans.nl/oauth";
        private static List<AnnouncementVM> _announcements;
        //private LocalObjectStorageHelper Helper { get; set; }
        private const string StorageKey = "AnnouncementStorage";

        public AnnouncementService()
        {
            //Helper = new LocalObjectStorageHelper();
        }

        public async Task<List<AnnouncementVM>> GetAnnouncements()
        {
            //await Task.CompletedTask;

            if (_announcements == null) {
                _announcements = new List<AnnouncementVM>();

                /*List<AnnouncementVM> result = await GetFromStorage();
                if (result == null)
                { // First time scenario
                    result = await Request();
                    SaveToStorage(result); // async
                }
                _announcements = result;*/

                _announcements = await Request();
            }
            return _announcements;
        }

        private async Task<List<AnnouncementVM>> Request()
        {
            XmlDocument data = await OAuth.GetInstance().RequestXML(base_url + "/bb/ann/", new List<string>(), Models.Enums.HttpMethod.GET);

            // The JSON has weird symbols in their messages. 
            //Announcements data = await OAuth.GetInstance().RequestJSON<Announcements>(base_url + "/bb/ann/", new List<string>(), Models.Enums.HttpMethod.GET);

            //List<AnnouncementVM> result = new List<AnnouncementVM>();
            //if (data != null && data.announcements != null && data.announcements.announcement != null)
            //result = data.announcements.announcement.Select(a => new AnnouncementVM(a as Announcement)).ToList();
            List<AnnouncementVM> result = ReadXMLDocument(data);

            return result;
        }


        private List<AnnouncementVM> ReadXMLDocument(XmlDocument doc)
        {
            List<AnnouncementVM> result = new List<AnnouncementVM>();

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
                        result.Add(new AnnouncementVM(item));
                    }
                }
                //result = Announcements.OrderByDescending(d => d.DateTime).ToList();
            }
            return result;
        }

        /*private async Task<List<AnnouncementVM>> GetFromStorage()
        {
            if (await Helper.FileExistsAsync(StorageKey))
            {
                List<AnnouncementVM> storage = await Helper.ReadFileAsync<List<AnnouncementVM>>(StorageKey, null); // default: null
                DeleteOldStorage(storage);
                return storage;
            }
            else
            {
                // First time 
                return null;
            }
        }
        private async void SaveToStorage(List<AnnouncementVM> results)
        {
            if (results == null || results.Count <= 0)
                return;

            if (await Helper.FileExistsAsync(StorageKey))
            {
                List<AnnouncementVM> storage = await GetFromStorage();
                if (storage != null)
                {
                    // Merge the results
                    foreach (var r in results)
                    {
                        storage.Add(r);
                    }
                    // Overwrite ?
                    await Helper.SaveFileAsync(StorageKey, storage);
                }
            }
            else
            {
                await Helper.SaveFileAsync(StorageKey, results);
            }
        }
        private void DeleteOldStorage(List<AnnouncementVM> storage)
        {
            // TODO
        }*/
    }
}
