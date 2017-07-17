using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using AvansApp.Models;
using AvansApp.ViewModels;
using AvansApp.Models.Enums;
using AvansApp.Models.ServerModels;

namespace AvansApp.Services.Pages
{
    public class ClassroomAvailabilityService
    {
        private const string base_url = "https://publicapi.avans.nl/oauth";

        public ClassroomAvailabilityService() { }


        public async Task<List<ClassroomAvailabilityVM>> GetClassroomAvailabilities(DateTime start, string classroomCode, ClassroomAvailabilityType type = ClassroomAvailabilityType.ALL, DateTime? end = null)
        {
            string av_type = GetAvailabilityType(type);
            string start_date = start.Year + "-" + start.Month + "-" + start.Day;
            string end_date = (end != null) ? end.Value.Year + "-" + end.Value.Month + "-" + end.Value.Day : "";
            classroomCode = classroomCode.ToUpper(); // Uppercase!
            
            //XmlDocument doc = await OAuth.GetInstance().RequestXML(base_url + "/lokaalbeschikbaarheid/", new List<string>() { "filter=" + classroomCode, "type=" + av_type, "start=" + start_date, (end != null) ? "end=" + end_date : "" }, HttpMethod.GET);

            List<ClassroomAvailability> data = await OAuth.GetInstance().RequestJSON<List<ClassroomAvailability>>(
                base_url + "/lokaalbeschikbaarheid/", 
                new List<string>() { "filter=" + classroomCode, "type=" + av_type, "start=" + start_date, (end != null) ? "end=" + end_date : "" },
                HttpMethod.GET
            );
            List<ClassroomAvailabilityVM> result = new List<ClassroomAvailabilityVM>();

            if (data != null)
            {
                for (int i=0; i < data.Count; i++)
                {
                    if (result.Count == 0)
                        result.Add(new ClassroomAvailabilityVM(data[i]));
                    else if (result[result.Count - 1].ScheduleTime != data[i].lesuur)
                        result.Add(new ClassroomAvailabilityVM(data[i]));
                }
            }

            //string _filename = "classroom_" + classroomCode + ".xml";
            //await SaveXML(doc, _filename);

            //List<ClassroomAvailabilityVM> _classrooms = ReadXMLDocument(doc);
            
            return result;
        }
        
        /*private List<ClassroomAvailabilityVM> ReadXMLDocument(XmlDocument doc)
        {
            List<ClassroomAvailabilityVM> _caVM = new List<ClassroomAvailabilityVM>();
            if (doc != null && doc.DocumentElement != null && doc.DocumentElement.ChildNodes.Count > 1)
            {
                var xml = doc.DocumentElement;

                foreach (var announcements in xml.ChildNodes)
                {
                    if (announcements.ChildNodes.Count > 0)
                    {
                        ClassroomAvailability item = new ClassroomAvailability();
                        foreach (var node in announcements.ChildNodes)
                        {
                            if (node.LocalName != null)
                            {
                                switch (node.LocalName.ToString())
                                {
                                    case "datum":
                                        DateTime date;
                                        if (DateTime.TryParse(node.InnerText.ToString(), out date))
                                            item.datum = date;
                                        else
                                            item.datum = new DateTime();
                                        break;
                                    case "lesuur":
                                        item.lesuur = node.InnerText.ToString();
                                        break;
                                    case "ruimte":
                                        item.ruimte = node.InnerText.ToString();
                                        break;
                                    case "grootte":
                                        item.grootte = node.InnerText.ToString();
                                        break;
                                    case "type":
                                        item.type = node.InnerText.ToString();
                                        break;
                                    case "bezet":
                                        if (node.InnerText.ToString() == "1")
                                            item.bezet = true;
                                        else
                                            item.bezet = false;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        if (_caVM.Count == 0)
                            _caVM.Add(new ClassroomAvailabilityVM(item));
                        else if (_caVM[_caVM.Count - 1].ScheduleTime != item.lesuur)
                            _caVM.Add(new ClassroomAvailabilityVM(item));
                    }
                }
                //_caVM = _caVM.OrderBy(d => d.Classroom).ToList();
            }
            else
            {
                // No result
                _caVM.Clear();
            }

            return _caVM;
        }*/
        
        public List<ClassroomVM> InitializeClassroom(List<ClassroomAvailabilityVM> classroomAvailabilities)
        {
            List<ClassroomVM> _classrooms = new List<ClassroomVM>();
            for (int i = 0; i < classroomAvailabilities.Count; i++)
            {
                if (i > 0 && classroomAvailabilities[i - 1].Classroom == classroomAvailabilities[i].Classroom)
                {
                    _classrooms[_classrooms.Count - 1].Availability.Add(new AvailabilityVM(new Availability() {
                        lesuur = classroomAvailabilities[i].ScheduleTime,
                        bezet = classroomAvailabilities[i].Occupied
                    }));
                    if (_classrooms[_classrooms.Count - 1].Availability.Count >= 15)
                    {
                        bool isOccupied = false;
                        foreach (AvailabilityVM a in _classrooms[_classrooms.Count - 1].Availability)
                        {
                            if (a.Occupied)
                            {
                                isOccupied = true;
                                break;
                            }
                        }
                        _classrooms[_classrooms.Count - 1].Occupied = isOccupied;
                    }
                }
                else
                {
                    Classroom classroom = new Classroom()
                    {
                        datum = classroomAvailabilities[i].DateTime,
                        grootte = classroomAvailabilities[i].ClassroomSpace,
                        ruimte = classroomAvailabilities[i].Classroom,
                        type = classroomAvailabilities[i].ClassroomType,
                        Availability = new List<Availability>() {
                            new Availability() {
                                lesuur = classroomAvailabilities[i].ScheduleTime,
                                bezet = classroomAvailabilities[i].Occupied
                            }
                        },
                        isBezet = default(bool)
                    };
                    _classrooms.Add(new ClassroomVM(classroom));
                }
            }
            return _classrooms;
        }

        private string GetAvailabilityType(ClassroomAvailabilityType type)
        {
            string availability_type = string.Empty;
            switch (type)
            {
                case ClassroomAvailabilityType.ALL:
                    availability_type = "all";
                    break;
                case ClassroomAvailabilityType.BUSY:
                    availability_type = "busy";
                    break;
                case ClassroomAvailabilityType.FREE:
                    availability_type = "free";
                    break;
                default:
                    availability_type = "all";
                    break;
            }
            return availability_type;
        }
        


        // TODO: Make offline??
        /*private async Task<List<ClassroomAvailabilityVM>> CheckFileExists(string classroomCode)
        {
            StorageFolder localfolder = ApplicationData.Current.LocalFolder;
            StorageFolder assetsfolder = await localfolder.CreateFolderAsync("Assets", CreationCollisionOption.OpenIfExists);
            StorageFolder CAfolder = await assetsfolder.CreateFolderAsync("ClassAvailability", CreationCollisionOption.OpenIfExists);

            string _filename = "classroom_" + classroomCode + ".xml";
            DateTime today = DateTime.Now;


            if (await CAfolder.TryGetItemAsync(_filename) != null)
            {
                // File detected! Open the file
                StorageFile textfile = await CAfolder.GetFileAsync(_filename);

                if (textfile.DateCreated.Year.Equals(today.Year) &&
                    textfile.DateCreated.Month.Equals(today.Month) &&
                    textfile.DateCreated.Day.Equals(today.Day))
                {
                    try
                    {
                        // Read the file
                        string contents = await FileIO.ReadTextAsync(textfile);

                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(contents);

                        List<ClassroomAvailabilityVM> r = ReadXMLDocument(doc);

                        return r;
                    }
                    catch (Exception Error)
                    {
                        System.Diagnostics.Debug.WriteLine(Error.Message);
                        return null;
                    }
                }
                else
                {
                    // Outdated
                    return null;
                }


            }
            else
            {
                // Nothing to see here...
                return null;
            }
        }*/
        /*private void DownloadClassrooms()
        {
            SearchBox.IsEnabled = false;
            SearchButton.IsEnabled = false;
            LoadingText.Visibility = Visibility.Visible;
            
            classroomCode = "O";

            if (main.CheckTokenExists("Settlement"))
            {
                string token = main.GetTokenFromVault("Settlement");
                if(token.ToLower() == "hoofdgebouw") {
                    classroomCode = "H";
                } else {
                    classroomCode = "O";
                }
            }

            

            List<ClassroomAvailability> SettlementList = new List<ClassroomAvailability>();


            List<ClassroomAvailability> file = await CheckFileExists(classroomCode);
            if(file != null)
            {
                // Do something

                SettlementList = file;
            }
            else
            {
                // Download & Save the file
                SettlementList = await GetClassrooms(DateTime.Now, classroomCode, AvailabilityType.ALL, DateTime.Now.AddDays(1));
                
            }
            
            //List<ClassroomAvailability> SettlementList = await GetClassrooms(DateTime.Now, classroomCode, AvailabilityType.ALL, DateTime.Now.AddDays(1));

            if(SettlementList != null)
            {
                ClassroomAvailabilities = new List<ClassroomAvailability>();
                foreach (ClassroomAvailability ca in SettlementList)
                {
                    ClassroomAvailabilities.Add(ca);
                }
                Classrooms = InitializeClassroom();
                
                ListClassroomAvailabilities.ItemsSource = new List<Classroom>();
                
                LoadingText.Visibility = Visibility.Collapsed;
                ListClassroomAvailabilities.Visibility = Visibility.Visible;
                NoResult.Visibility = Visibility.Collapsed;
                SearchBox.IsEnabled = true;
                SearchButton.IsEnabled = true;
            }
            else
            {
                ClassroomAvailabilities = new List<ClassroomAvailability>();
                ListClassroomAvailabilities.ItemsSource = ClassroomAvailabilities;

                LoadingText.Visibility = Visibility.Collapsed;
                ListClassroomAvailabilities.Visibility = Visibility.Collapsed;
                NoResult.Visibility = Visibility.Visible;
                SearchBox.IsEnabled = true;
                SearchButton.IsEnabled = true;
            }
        }*/
        /*private async Task SaveXML(XmlDocument doc, string filename)
        {
            try
            {
                StorageFolder localfolder = ApplicationData.Current.LocalFolder;
                StorageFolder assetsfolder = await localfolder.CreateFolderAsync("Assets", CreationCollisionOption.OpenIfExists);
                StorageFolder CAfolder = await assetsfolder.CreateFolderAsync("ClassAvailability", CreationCollisionOption.OpenIfExists);
                
                StorageFile file = await CAfolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

                await doc.SaveToFileAsync(file);
            }
            catch (Exception Error) { Debug.WriteLine(Error.Message); }
        }*/

    }
}
