using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvansApp.ViewModels;
using AvansApp.Models.ServerModels;
using AvansApp.Models;
using AvansApp.Helpers;
using System.Globalization;

namespace AvansApp.Services.Pages
{
    public class ProfileService
    {
        private ProfileVM User { get; set; }
        private double? AverageGrade { get; set; }
        private double? TotalEC { get; set; }
        private int? AmountPassingGrades { get; set; }
        private int? AmountFailingGrades { get; set; }

        public ProfileService()
        {
            User = null;
            AverageGrade = null;
            TotalEC = null;
            AmountPassingGrades = null;
            AmountFailingGrades = null;
        }

        public async Task<ProfileVM> GetUserAsync()
        {
            if (User != null)
                return User;

            // Create Test User
            User = new ProfileVM
            {
                Name = "John Doe",
                Title = "Student",
                Emailadres = "johndoe@email.nl",
                ProfilePicture = "ms-appx:///Assets/StoreLogo.png"
            };
            EmployeeImage profileImage = null;

            Student[] s = await OAuth.GetInstance().RequestJSON<Student[]>("https://publicapi.avans.nl/oauth/studentnummer/", new List<string>(), Models.Enums.HttpMethod.GET);

            if (s != null && s[0] != null)
            {
                Student student = s[0];
                People people = await OAuth.GetInstance().RequestJSON<People>("https://publicapi.avans.nl/oauth/people/" + student.inlognaam, new List<string>(), Models.Enums.HttpMethod.GET);
                if (people != null)
                {
                    string userimage = await OAuth.GetInstance().RequestRaw("https://publicapi.avans.nl/oauth/medewerkersgids/image/" + student.inlognaam, new List<string>(), Models.Enums.HttpMethod.GET);

                    if (!string.IsNullOrEmpty(userimage))
                    {
                        try
                        {
                            userimage = userimage.Substring(userimage.IndexOf('{')); // Server gives us a weird json object. This fixes it.

                            profileImage = Newtonsoft.Json.JsonConvert.DeserializeObject<EmployeeImage>(userimage);
                            User.EmployeeImage = profileImage;

                            if (profileImage != null)
                            {
                                User.EmployeeImage.bitmap = await profileImage.Base64StringToBitmap(profileImage.image);
                            }
                        }
                        catch (Exception Error)
                        {
                            System.Diagnostics.Debug.WriteLine(Error.Message);
                        }
                    }

                    // Real User
                    User = new ProfileVM
                    {
                        Firstname = people.name.givenName,
                        Surname = people.name.familyName,
                        Name = people.name.formatted,
                        Login = student.inlognaam,
                        Studentnummer = student.studentnummer,
                        Emailadres = people.emails.FirstOrDefault(),
                        Title = people.title,
                        ProfilePicture = "ms-appx:///Assets/StoreLogo.png",
                        EmployeeImage = profileImage
                    };
                }
            }

            return User;
        }

        private async Task CalculateResults()
        {
            List<ResultVM> results = await Singleton<ResultService>.Instance.GetResults();
            List<ResultVM> newResults = new List<ResultVM>();

            double sumResult = 0.0;
            int amountResults = 0;
            int amountPassing = 0;
            int amountFailing = 0;

            foreach (ResultVM r in results)
            {
                ResultVM item = InArray(r, ref newResults);
                if (item != null)
                {
                    if (r.Voldoende.ToUpper() == "J" && item.Voldoende.ToUpper() == "J")
                    {
                        string newItemMark = r.Resultaat;
                        newItemMark = newItemMark.Replace(",", ".");

                        if (double.TryParse(newItemMark, NumberStyles.Any, CultureInfo.InvariantCulture, out double newMark))
                        {
                            string itemMark = item.Resultaat;
                            itemMark = itemMark.Replace(",", ".");

                            if (double.TryParse(itemMark, NumberStyles.Any, CultureInfo.InvariantCulture, out double mark))
                            {
                                if (newMark > mark)
                                {
                                    newResults.Remove(item);
                                    newResults.Add(r);
                                }
                            }
                        }
                        else if (r.Resultaat.ToUpper() == "G" || r.Resultaat.ToUpper() == "U") // just in case
                        {
                            newResults.Remove(item);
                            newResults.Add(r);
                        }
                    }
                    else if (r.Voldoende == "J" && item.Voldoende != "J")
                    {
                        newResults.Remove(item);
                        newResults.Add(r);
                    }
                }
                else
                {
                    newResults.Add(r);
                }

                if (r.Voldoende.ToUpper() == "J")
                {
                    amountPassing++;
                }
                else
                {
                    amountFailing++;
                }
            }

            double sumEC = 0.0;

            foreach (ResultVM r in newResults)
            {
                string result = r.Resultaat;
                result = result.Replace(",", ".");

                if (double.TryParse(result, NumberStyles.Any, CultureInfo.InvariantCulture, out double rs))
                {
                    sumResult += rs;
                    amountResults++;
                }
                string ecStr = r.AantalEC;
                ecStr = ecStr.Replace(",", ".");
                if (double.TryParse(ecStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double ec))
                {
                    sumEC += ec;
                }
            }
            
            AverageGrade = (amountResults > 0) ? (sumResult / amountResults) : 0;
            TotalEC = sumEC;
            AmountPassingGrades = amountPassing;
            AmountFailingGrades = amountFailing;
        }

        public async Task<int> GetAmountPassingGrades()
        {
            if (AmountPassingGrades == null)
                await CalculateResults();

            return AmountPassingGrades.Value;
        }
        public async Task<int> GetAmountFailingGrades()
        {
            if (AmountFailingGrades == null)
                await CalculateResults();
            
            return AmountFailingGrades.Value;
        }
        public async Task<double> GetAverageGrade()
        {
            if (AverageGrade == null)
                await CalculateResults();
            
            return AverageGrade.Value;
        }

        public async Task<double> GetTotalEC()
        {
            if (TotalEC == null)
                await GetAverageGrade();

            return TotalEC.Value;
        }

        private ResultVM InArray(ResultVM item, ref List<ResultVM> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (item.CursusCode == list[i].CursusCode && 
                    item.ToetsOmschrijving == list[i].ToetsOmschrijving)
                {
                    return list[i];
                }
            }
            return null;
        }
    }
}
