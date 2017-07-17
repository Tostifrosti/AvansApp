using AvansApp.Models.ServerModels;

namespace AvansApp.ViewModels
{
    public class EmployeeVM
    {
        private Employee _employee;

        public EmployeeVM()
        {
            _employee = new Employee();
        }
        public EmployeeVM(Employee e)
        {
            _employee = e;
        }

        //General
        public string Email
        {
            get { return _employee.mail; }
            set { _employee.mail = value; }
        }
        public string OU
        {
            get { return _employee.ou; }
            set { _employee.ou = value; }
        }
        public string DisplayName
        {
            get { return _employee.displayname; }
            set { _employee.displayname = value; }
        }
        public string PTC
        {
            get { return _employee.ptc; }
            set { _employee.ptc = value; }
        }
        public EmployeeImage ProfilePicture
        {
            get { return _employee.ProfilePicture; }
            set { _employee.ProfilePicture = value; }
        }
        public string PrivateNumber
        {
            get { return _employee.telefoonextern; }
            set { _employee.telefoonextern = value; }
        }
        public string WorkNumber
        {
            get { return _employee.toestel; }
            set { _employee.toestel = value; }
        }
        public string Login
        {
            get { return _employee.inlognaam; }
            set { _employee.inlognaam = value; }
        }
        
        // EN
        public string FirstName
        {
            get { return _employee.givenname; }
            set { _employee.givenname = value; }
        }
        public string Surname
        {
            get { return _employee.sn; }
            set { _employee.sn = value; }
        }
        public string FullName
        {
            get { return this.FirstName + " " + this.Surname; }
        }
        public string Title
        {
            get { return _employee.title; }
            set { _employee.title = value; }
        }
        public string Location
        {
            get { return _employee.location; }
            set { _employee.location = value; }
        }
        public string Room
        {
            get { return _employee.room; }
            set { _employee.room = value; }
        }
        public string Workdays
        {
            get { return _employee.workdays; }
            set { _employee.workdays = value; }
        }
        


        //NL
        public string Voornaam
        {
            get { return _employee.voornaam; }
            set { _employee.voornaam = value; }
        }
        public string Achternaam
        {
            get { return _employee.achternaam; }
            set { _employee.achternaam = value; }
        }
        public string VolledigeNaam
        {
            get { return this.Voornaam + " " + this.Achternaam; }
        }
        public string Titel
        {
            get { return _employee.titel; }
            set { _employee.titel = value; }
        }
        public string Lokatie
        {
            get { return _employee.lokatie; }
            set { _employee.lokatie = value; }
        }
        public string Kamer
        {
            get { return _employee.kamer; }
            set { _employee.kamer = value; }
        }
        public string Werkdagen
        {
            get { return _employee.werkdagen; }
            set { _employee.werkdagen = value; }
        }

    }
}
