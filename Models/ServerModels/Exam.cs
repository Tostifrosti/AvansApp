using System;

namespace AvansApp.Models.ServerModels
{
    public class Exam
    {
        public string TentamenRoosterStudentID { get; set; }
        public int studentnummer { get; set; }
        public string naam { get; set; }
        public string cursus { get; set; }
        public string Toetsomschrijving { get; set; }
        public DateTime sortbegin { get; set; }
        public string Begindatum { get; set; }
        public string Begintijd { get; set; } // Tijd
        public string Einddatum { get; set; } // Tijd
        public string Lokaal { get; set; }
        public string Locatie { get; set; }
    }
}
