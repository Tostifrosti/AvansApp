using System;

namespace AvansApp.Models.ServerModels
{
    public class Result
    {
        /*
         * Watch out!
         * Properties are Case-Sensitive!
         */
        public string honorairePunten { get; set; }
        public string voldoende { get; set; } // J (= Ja), N (= Nee)
        public string cursuscode { get; set; }
        public string langeNaamCursus { get; set; } // Volledige naam
        public int studentnummer { get; set; }
        public string status { get; set; } // D (= Definitief), C (= Concept)
        public string resultaat { get; set; } // cijfer (= decimaal/int), VR (= Vrijstelling), VLD (= Voldoende), NVLD (= ), V (= )
        public string docent { get; set; }
        public string onderwerp { get; set; }
        public string mutatiedatumTekst { get; set; } // Datum Laatst aangepast Tekst vorm
        public DateTime mutatiedatum { get; set; } // Datum Laatst aangepast
        public string korteNaamCusus { get; set; } // 
        public string toetsdatumTekst { get; set; } // Datum Tentamen
        public DateTime toetsdatum { get; set; } // Datum Tentamen
        public string toetsomschrijving { get; set; }
        public string punten { get; set; } // Aantal EC (decimal / int)
        public string weging { get; set; } // Weging toets (decimal / int)
    }
}
