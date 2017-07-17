using AvansApp.Models.ServerModels;

namespace AvansApp.Models
{
    public class Profile
    {
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public int StudentNumber { get; set; }
        public string Emailadres { get; set; }
        public string ProfilePicture { get; set; }
        public string Title { get; set; }
        public EmployeeImage EmployeeImage { get; set; }
    }
}
