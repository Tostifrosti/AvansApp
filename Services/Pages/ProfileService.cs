using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvansApp.ViewModels;
using AvansApp.Models.ServerModels;
using AvansApp.Models;

namespace AvansApp.Services.Pages
{
    public class ProfileService
    {
        private ProfileVM User { get; set; }

        public ProfileService()
        {
            User = null;
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

            if (s != null)
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
    }
}
