using System.Collections.Generic;

namespace AvansApp.Models.ServerModels
{
    public class A
    {
        /*
         * Watch out!
         * Properties are Case-Sensitive!
         */
        public List<Announcement> announcement { get; set; }
    }

    public class Announcements
    {
        /*
         * Watch out!
         * Properties are Case-Sensitive!
         */
        public A announcements;
    }
}
