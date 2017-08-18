using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Data.Xml.Dom;

using AvansApp.Models;
using AvansApp.Helpers;
using AvansApp.ViewModels;
using AvansApp.Models.ServerModels;

namespace AvansApp.Services.Pages
{
    public class DisruptionService
    {
        private const string base_url = "https://publicapi.avans.nl/oauth";
        private static List<DisruptionItemVM> Items { get; set; }
        private const string StorageKey = "DisruptionStorage";
        private DateTime refreshTime;

        public DisruptionService()
        {
            refreshTime = DateTime.Now;
            Items = null;
        }

        public async Task<List<DisruptionItemVM>> GetDisruptions()
        {
            if (Items == null || refreshTime > DateTime.Now.AddMinutes(-1))
            {
                refreshTime = DateTime.Now;

                Items = new List<DisruptionItemVM>();

                List<DisruptionItem> storage = await GetFromStorage();
                List<DisruptionItem> newItems = await Request();

                if (await CompareNewDisruptionsAsync(newItems) <= 0)
                {
                    storage = await GetFromStorage();

                    foreach (DisruptionItem d in storage)
                        Items.Add(new DisruptionItemVM(d));

                    return Items;
                }
                else
                {
                    // New results!
                    storage = await GetFromStorage();

                    foreach (DisruptionItem d in storage)
                        Items.Add(new DisruptionItemVM(d));

                    return Items;
                }
            }
            return Items;
        }

        public async Task<List<DisruptionItem>> Request()
        {
            List<DisruptionItem> result = new List<DisruptionItem>();
            XmlDocument doc = await OAuth.GetInstance().RequestXML("https://storing.avans.nl/rss.php", new List<string>() { "lang=nl" }, Models.Enums.HttpMethod.GET); // XML only

            if (doc != null && doc.DocumentElement != null && doc.DocumentElement.ChildNodes.Count > 1)
            {
                var xml = doc.DocumentElement;

                foreach (var childnodes in xml.ChildNodes)
                {
                    if (childnodes.ChildNodes.Count > 0)
                    {
                        foreach (var node in childnodes.ChildNodes)
                        {
                            if (node.LocalName != null && node.LocalName.ToString() == "item") // we only want the items
                            {
                                DisruptionItem item = new DisruptionItem();
                                foreach (var nodeitem in node.ChildNodes)
                                {
                                    if (nodeitem.LocalName != null)
                                    {
                                        switch (nodeitem.LocalName.ToString())
                                        {
                                            case "title":
                                                item.Title = nodeitem.InnerText.ToString();
                                                break;
                                            case "link":
                                                item.Link = nodeitem.InnerText.ToString();
                                                break;
                                            case "guid":
                                                item.GuId = nodeitem.InnerText.ToString();
                                                break;
                                            case "pubDate":
                                                DateTime date;
                                                if (DateTime.TryParse(nodeitem.InnerText.ToString(), out date))
                                                    item.PublicationDate = date;
                                                else
                                                    item.PublicationDate = new DateTime();
                                                break;
                                            case "type":
                                                int type;
                                                if (int.TryParse(nodeitem.InnerText.ToString(), out type))
                                                    item.Type = type;
                                                else
                                                    item.Type = -1;
                                                break;
                                            case "description":
                                                item.Description = nodeitem.InnerText.ToString();
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
                }
            }
            return result;
        }
        public async Task<int> CompareNewDisruptionsAsync(List<DisruptionItem> newItems)
        {
            if (newItems == null || newItems.Count <= 0)
                return 0;

            List<DisruptionItem> storage = await GetFromStorage();
            int foundNewItems = 0;

            if (storage == null || storage.Count <= 0)
            {
                // Storage is empty
                foreach (DisruptionItem d in newItems)
                {
                    storage.Add(d);
                }
                // No need to sort
                await SaveToStorage(storage);

                return newItems.Count;
            }
            else
            {
                List<DisruptionItem> items = new List<DisruptionItem>();
                int temp = -1;

                // Comprare old & new disruptions
                for (int i = (newItems.Count - 1); i >= 0; i--)
                {
                    temp = -1;
                    for (int j = (storage.Count - 1); j >= 0; j--)
                    {
                        if (Compare(newItems[i], storage[j]))
                        {
                            temp = j;
                            break;
                        }
                    }

                    if (temp >= 0)
                    {
                        items.Add(newItems[i]);
                        newItems.RemoveAt(i);
                    }
                }

                // Add new disruptions
                foreach (DisruptionItem item in newItems)
                {
                    foundNewItems++;
                    items.Add(item);
                }

                // No need to sort
                await SaveToStorage(items);
            }

            return foundNewItems;
        }
        private async Task<List<DisruptionItem>> GetFromStorage()
        {
            if (ApplicationData.Current.LocalFolder.FileExists(StorageKey))
            {
                return await ApplicationData.Current.LocalFolder.ReadAsync<List<DisruptionItem>>(StorageKey);
            }
            else
            {
                // First time scenario
                return new List<DisruptionItem>();
            }
        }
        private async Task SaveToStorage(List<DisruptionItem> items)
        {
            if (items == null || items.Count <= 0)
                return;

            await ApplicationData.Current.LocalFolder.SaveAsync(StorageKey, items);
        }
        public bool Compare(DisruptionItem a, DisruptionItem b)
        {
            if (a == null || b == null)
                return false;

            if (a.GuId == b.GuId)
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
