using System;
using System.Collections.Generic;

namespace AvansApp.Models.ServerModels
{
    public class Classroom
    {
        public DateTime datum { get; set; }
        public string ruimte { get; set; }
        public string grootte { get; set; }
        public string type { get; set; }
        public bool isBezet { get; set; }
        public List<Availability> Availability { get; set; }
    }
}
