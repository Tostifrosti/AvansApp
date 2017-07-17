using System.Collections.Generic;

namespace AvansApp.Models.ServerModels
{
    public class People
    {
        /*
         * Watch out!
         * Properties are Case-Sensitive!
         */
        public string nickname { get; set; }
        public List<string> emails { get; set; }
        public string voot_membership_role { get; set; }
        public string id { get; set; }
        public Name name { get; set; }
        public List<string> tags { get; set; }
        public Account accounts { get; set; }
        public string organizations { get; set; }
        public string title { get; set; }
        public string cardnumber { get; set; }
        public List<string> phoneNumbers { get; set; }
        public string location { get; set; }
        public bool employee { get; set; }
        public bool student { get; set; }
    }
}
