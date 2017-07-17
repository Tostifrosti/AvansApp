using System;

namespace AvansApp.Models.ServerModels
{
    public class Announcement
    {
        /*
         * Watch out!
         * Properties are Case-Sensitive!
         */
        public string title { get; set; }
        public string course { get; set; }
        public string link { get; set; }
        public DateTime datetime { get; set; }
        public string message { get; set; }
    }
}
