using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.Data.Xml.Dom;

using AvansApp.Models;
using AvansApp.Models.ServerModels;
using AvansApp.ViewModels;

namespace AvansApp.Services.Pages
{
    public class DisruptionService
    {
        private const string base_url = "https://publicapi.avans.nl/oauth";

        public DisruptionService()
        {

        }
        public async Task<List<DisruptionItemVM>> GetDisruptions()
        {
            List<DisruptionItemVM> result = new List<DisruptionItemVM>();
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
                                } // end-foreach
                                result.Add(new DisruptionItemVM(item));
                            }
                        } // end-foreach
                    }
                }
                //result = result.OrderByDescending(d => d.PublicationDate).ToList();
            }
            return result;
        }
    }
}
