using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.Data.Xml.Dom;

using AvansApp.ViewModels;
using AvansApp.Models;
using AvansApp.Models.ServerModels;

namespace AvansApp.Services.Pages
{
    public class ExamService
    {
        private const string base_url = "https://publicapi.avans.nl/oauth";
        public bool ExamSubsriptionToken { get; private set; }

        public ExamService()
        {
            ExamSubsriptionToken = false;
        }

        public async Task<List<ExamVM>> GetExams()
        {
            List<ExamVM> result = new List<ExamVM>();

            XmlDocument doc = await OAuth.GetInstance().RequestXML(base_url + "/tentamenrooster/", new List<string>(), Models.Enums.HttpMethod.GET);
            //List<Exam> data = await OAuth.GetInstance().RequestJSON<List<Exam>>(base_url + "/tentamenrooster", new List<string>(), Models.Enums.HttpMethod.GET); // Test when exams are available

            if (doc != null && doc.DocumentElement != null && doc.DocumentElement.ChildNodes.Count > 1) // Check if Result
            {
                var xml = doc.DocumentElement;

                foreach (var exams in xml.ChildNodes)
                {
                    if (exams.ChildNodes.Count > 0)
                    {
                        Exam item = new Exam();
                        foreach (var node in exams.ChildNodes)
                        {
                            if (node.LocalName != null)
                            {
                                if (node.LocalName.ToString().ToLower() == "inschrijven")
                                {
                                    ExamSubsriptionToken = true;
                                    item = null;
                                    break;
                                }

                                switch (node.LocalName.ToString())
                                {
                                    case "TentamenRoosterStudentID":
                                        item.TentamenRoosterStudentID = node.InnerText.ToString();
                                        break;
                                    case "studentnummer":
                                        item.studentnummer = int.Parse(node.InnerText.ToString());
                                        break;
                                    case "naam":
                                        item.naam = node.InnerText.ToString();
                                        break;
                                    case "cursus":
                                        item.cursus = node.InnerText.ToString();
                                        break;
                                    case "Toetsomschrijving":
                                        item.Toetsomschrijving = node.InnerText.ToString();
                                        break;
                                    case "sortbegin":
                                        DateTime date;
                                        if (DateTime.TryParse(node.InnerText.ToString(), out date))
                                            item.sortbegin = date;
                                        else
                                            item.sortbegin = new DateTime(); //node.InnerText.ToString()
                                        break;
                                    case "Begindatum":
                                        item.Begindatum = node.InnerText.ToString();
                                        break;
                                    case "Begintijd":
                                        item.Begintijd = node.InnerText.ToString();
                                        break;
                                    case "Einddatum":
                                        item.Einddatum = node.InnerText.ToString();
                                        break;
                                    case "Lokaal":
                                        item.Lokaal = node.InnerText.ToString();
                                        break;
                                    case "Locatie":
                                        item.Locatie = node.InnerText.ToString();
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        if (item != null)
                            result.Add(new ExamVM(item));
                    }
                }
                //result = result.OrderByDescending(d => d.DateTime).ToList();
            }
            return result;
        }

        public bool Compare(Exam a, Exam b)
        {
            if (a == null || b == null)
                return false;

            if (a.naam == b.naam &&
                a.studentnummer == b.studentnummer &&
                a.Begindatum == b.Begindatum)
            {
                return true;
            }

            return false;
        }
        public bool Compare(ExamVM a, ExamVM b)
        {
            if (a == null || b == null)
                return false;

            if (a.StudentName == b.StudentName &&
                a.StudentNumber == b.StudentNumber &&
                a.StartDate == b.StartDate)
            {
                return true;
            }

            return false;
        }
    }
}
