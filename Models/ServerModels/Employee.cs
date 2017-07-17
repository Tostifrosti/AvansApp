namespace AvansApp.Models.ServerModels
{
    public class Employee
    {
        /*
         * Watch out!
         * Properties are Case-Sensitive!
         */

        // EN
        public string ptc { get; set; } // Inlognaam?
        public string givenname { get; set; } // Voornaam
        public string sn { get; set; } // Achternaam
        public string mail { get; set; } // Avans Mail
        public string title { get; set; } // Titel bezigheid
        public string ou { get; set; } // Onderwijs afkorting ??
        public string location { get; set; } // Locatie afkorting
        public string room { get; set; } // Kamer nr
        public string workdays { get; set; } // Ma, Di, Wo, Do, Vr
        public string displayname { get; set; } // Volledige naam

        // NL
        public string inlognaam { get; set; }
        public string voornaam { get; set; }
        public string achternaam { get; set; }
        public string telefoonextern { get; set; } // Privé Telefoon nr
        public string toestel { get; set; } // Intern Telefoon nr
        public string lokatie { get; set; }
        public string kamer { get; set; }
        public string werkdagen { get; set; }
        public string titel { get; set; }

        public EmployeeImage ProfilePicture { get; set; }
    }
}
