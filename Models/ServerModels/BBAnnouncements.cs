using System;

namespace AvansApp.Models.ServerModels
{
    public class BBAnnouncement
    {
        /*
         * Watch out!
         * Properties are Case-Sensitive!
         */
        public string courseID { get; set; }
        public string batchID { get; set; }
        public string courseName { get; set; }
        public string title { get; set; }
        public DateTime datetime { get; set; }
        public string link { get; set; }
    }
}
