using System;
using System.Collections.Generic;

namespace AvansApp.Models.ServerModels
{
    public class Schedule
    {
        public int id { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string starttijd { get; set; } // Tekstuele weergave
        public string eindtijd { get; set; } // Tekstuele weergave
        public string vak { get; set; } // Rooster code
        public string ruimte { get; set; } // Het gene waardoor je het rooster hebt opgevraagd
        public string groep { get; set; } // Het gene waardoor je het rooster hebt opgevraagd
        public string docent { get; set; } // Het gene waardoor je het rooster hebt opgevraagd
        public string details { get; set; }
        public string uurlabel { get; set; }
        public string weeklabel { get; set; }
        public DateTime datum { get; set; }
    }
}
