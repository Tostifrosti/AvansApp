using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;

using AvansApp.Models;
using AvansApp.Models.ServerModels;
using AvansApp.ViewModels;
using AvansApp.Helpers;
using Windows.Storage;

namespace AvansApp.Services.Pages
{
    public class ResultService
    {
        private const string base_url = "https://publicapi.avans.nl/oauth";
        private static List<ResultVM> Results { get; set; }
        private const string StorageKey = "ResultStorage";
        private DateTime refreshTime;
        
        public ResultService()
        {
            refreshTime = DateTime.Now;
            Results = null;
        }

        public async Task<List<ResultVM>> GetResults()
        {
            if (Results == null || refreshTime > DateTime.Now.AddMinutes(-1)) {
                refreshTime = DateTime.Now;

                Results = new List<ResultVM>();

                List<Result> storage = await GetFromStorage();
                List<Result> newResults = await RequestResults();

                if (await CompareNewResultsAsync(newResults) <= 0)
                {
                    foreach (Result r in storage)
                        Results.Add(new ResultVM(r));

                    return Results;
                }
                else
                {
                    // New results!
                    storage = await GetFromStorage();

                    foreach (Result r in storage)
                        Results.Add(new ResultVM(r));

                    return Results;
                }
            }
            return Results;
        }

        public async Task<List<Result>> RequestResults()
        {
            List<Result> results = new List<Result>();
            XmlDocument doc = await OAuth.GetInstance().RequestXML(base_url + "/resultaten/v2/", new List<string>(), Models.Enums.HttpMethod.GET);
            
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
                        results.Add(r);
                    }
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

        public async Task<int> CompareNewResultsAsync(List<Result> newResults)
        {
            if (newResults == null || newResults.Count <= 0)
                return 0;

            List<Result> storage = await GetFromStorage();
            int foundNewResults = 0;

            if (storage == null || storage.Count <= 0)
            {
                // Storage is empty
                foreach (Result r in newResults)
                {
                    storage.Add(r);
                }
                // No need to sort
                await SaveToStorage(storage);

                return newResults.Count;
            }
            else
            {
                foreach (Result nr in newResults)
                {
                    int temp = -1;
                    for (int i=0; i < storage.Count; i++)
                    {
                        if (CompareResult(nr, storage[i]))
                        {
                            temp = i;
                            break;
                        }
                    }

                    if (temp < 0)
                    {
                        foundNewResults++;
                        storage.Add(nr);
                    } else {
                        storage[temp] = nr; // Update result
                    }
                }

                // No need to sort
                await SaveToStorage(storage);
            }

            return foundNewResults;
        }
        private async Task<List<Result>> GetFromStorage()
        {
            if (ApplicationData.Current.LocalFolder.FileExists(StorageKey))
            {
                return await ApplicationData.Current.LocalFolder.ReadAsync<List<Result>>(StorageKey);
            }
            else
            {
                // First time scenario
                return new List<Result>();
            }
        }
        private async Task SaveToStorage(List<Result> results)
        {
            if (results == null || results.Count <= 0)
                return;

            await ApplicationData.Current.LocalFolder.SaveAsync(StorageKey, results);
        }

        public bool CompareResult(Result r1, Result r2)
        {
            if (r1 == null || r2 == null)
                return false;

            if (r1.cursuscode == r2.cursuscode &&
                r1.studentnummer == r2.studentnummer)
            {
                return true;
            }

            return false;
        }

        public void DeleteResultStorage()
        {
            ApplicationData.Current.LocalFolder.DeleteAsync(StorageKey);
        }

    }
}
