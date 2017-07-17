using System;

namespace AvansApp.Models.ServerModels
{
    public class ClassroomAvailability
    {
        public DateTime datum { get; set; }
        public string lesuur { get; set; }
        public string ruimte { get; set; }
        public string grootte { get; set; }
        public string type { get; set; }
        public bool bezet { get; set; } // 0 or 1
    }
}
