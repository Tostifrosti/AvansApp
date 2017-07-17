using AvansApp.Models;
using AvansApp.Models.ServerModels;

namespace AvansApp.ViewModels
{
    public class ProfileVM
    {
        private Profile _profile;
        
        public ProfileVM()
        {
            this._profile = new Profile();
        }

        public ProfileVM(Profile p)
        {
            this._profile = p;
        }
        public string Firstname
        {
            get { return this._profile.Firstname; }
            set { this._profile.Firstname = value; }
        }
        public string Surname
        {
            get { return this._profile.Surname; }
            set { this._profile.Surname = value; }
        }
        public string Name
        {
            get { return this._profile.Name; }
            set { this._profile.Name = value; }
        }
        public string Fullname
        {
            get { return this.Firstname + " " + this.Surname; }
        }
        public string Login
        {
            get { return this._profile.Login; }
            set { this._profile.Login = value; }
        }
        public int Studentnummer
        {
            get { return this._profile.StudentNumber; }
            set { this._profile.StudentNumber = value; }
        }

        public string Emailadres
        {
            get { return this._profile.Emailadres; }
            set { this._profile.Emailadres = value; }
        }
        public string ProfilePicture
        {
            get { return this._profile.ProfilePicture; }
            set { this._profile.ProfilePicture = value; }
        }
        public string Title
        {
            get { return this._profile.Title; }
            set { this._profile.Title = value; }
        }
        public EmployeeImage EmployeeImage
        {
            get { return this._profile.EmployeeImage; }
            set { this._profile.EmployeeImage = value; }
        }
    }
}
