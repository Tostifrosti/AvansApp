using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;

using AvansApp.Models;
using AvansApp.Models.ServerModels;
using AvansApp.ViewModels;

namespace AvansApp.Services.Pages
{
    public class ResultService
    {
        private const string base_url = "https://publicapi.avans.nl/oauth";
        private static List<ResultVM> _results;
        //private LocalObjectStorageHelper Helper { get; set; }
        private const string StorageKey = "ResultStorage";
        
        public ResultService()
        {
            //Helper = new LocalObjectStorageHelper();
        }

        public async Task<List<ResultVM>> GetResults()
        {
            if (_results == null) {
                _results = new List<ResultVM>();

                /*List<ResultVM> result = await GetFromStorage();
                if (result == null) { // First time scenario
                    result = await Request();
                    SaveToStorage(result); // async
                }
                _results = result;*/
                _results = await RequestResults();
            }
            return _results;
        }

        public async Task<List<ResultVM>> RequestResults()
        {
            List<ResultVM> results = new List<ResultVM>();
            XmlDocument doc = await OAuth.GetInstance().RequestXML(base_url + "/resultaten/v2/", new List<string>(), Models.Enums.HttpMethod.GET);

           // List<Result> data = await OAuth.GetInstance().RequestJSON<List<Result>>(base_url + "/resultaten/v2/", new List<string>(), Models.Enums.HttpMethod.GET);

            if (doc != null)
            {
                var envelope = doc.DocumentElement;
                if (envelope != null && envelope.ChildNodes.Count > 1)
                {
                    var body = envelope.LastChild;
                    var resultatenRespone = body.FirstChild;
                    var result = resultatenRespone.FirstChild;

                    foreach (var parent in result.ChildNodes)
                    {
                        Result r = new Result();
                        foreach (var node in parent.ChildNodes)
                        {
                            switch (node.LocalName.ToString())
                            {
                                case "honorairePunten":
                                    r.honorairePunten = node.InnerText.ToString();
                                    break;
                                case "voldoende":
                                    r.voldoende = node.InnerText.ToString();
                                    break;
                                case "cursuscode":
                                    r.cursuscode = node.InnerText.ToString();
                                    break;
                                case "langeNaamCursus":
                                    r.langeNaamCursus = node.InnerText.ToString();
                                    break;
                                case "studentnummer":
                                    int studentnr;
                                    if (int.TryParse(node.InnerText.ToString(), out studentnr))
                                        r.studentnummer = studentnr;
                                    else
                                        r.studentnummer = -1;
                                    break;
                                case "status":
                                    r.status = node.InnerText.ToString();
                                    break;
                                case "resultaat":
                                    r.resultaat = node.InnerText.ToString();
                                    break;
                                case "docent":
                                    r.docent = node.InnerText.ToString();
                                    break;
                                case "onderwerp":
                                    r.onderwerp = node.InnerText.ToString();
                                    break;
                                case "mutatiedatumTekst":
                                    r.mutatiedatumTekst = node.InnerText.ToString();
                                    break;
                                case "mutatiedatum":
                                    DateTime date;
                                    if (DateTime.TryParse(node.InnerText.ToString(), out date))
                                        r.mutatiedatum = date;
                                    else
                                        r.mutatiedatum = new DateTime(); //node.InnerText.ToString()
                                    break;
                                case "korteNaamCusus":
                                    r.korteNaamCusus = node.InnerText.ToString();
                                    break;
                                case "toetsdatumTekst":
                                    r.toetsdatumTekst = node.InnerText.ToString();
                                    break;
                                case "toetsdatum":
                                    DateTime d;
                                    if (DateTime.TryParse(node.InnerText.ToString(), out d))
                                        r.toetsdatum = d;
                                    else
                                        r.toetsdatum = new DateTime(); //node.InnerText.ToString()
                                    break;
                                case "toetsomschrijving":
                                    r.toetsomschrijving = node.InnerText.ToString();
                                    break;
                                case "punten":
                                    r.punten = node.InnerText.ToString();
                                    break;
                                case "weging":
                                    r.weging = node.InnerText.ToString();
                                    break;
                                default:
                                    break;
                            }
                        }
                        results.Add(new ResultVM(r));
                    }

                    //results = new List<ResultVM>(results.OrderByDescending(d => d.ToetsDatum));
                    
                }
                else
                {
                    // No result
                    results.Clear();
                }
            }
            else
            {
                // Something went wrong?
                results.Clear();
            }
            return results;
        }

        public bool CompareNewResults(List<ResultVM> newResults)
        {
            return false;
        }
        /*private async Task<List<ResultVM>> GetFromStorage()
        {
            if (await Helper.FileExistsAsync(StorageKey))
            {
                List<ResultVM> storage = await Helper.ReadFileAsync<List<ResultVM>>(StorageKey, null); // default: null
                return storage;
            }
            else
            {
                // First time 
                return null;
            }
        }
        private async void SaveToStorage(List<ResultVM> results)
        {
            if (results == null || results.Count <= 0)
                return;

            if (await Helper.FileExistsAsync(StorageKey))
            {
                List<ResultVM> storage = await GetFromStorage();
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
        }*/
    }
}
