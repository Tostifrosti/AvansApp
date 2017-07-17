using System;

namespace AvansApp.Models.ServerModels
{
    public class DisruptionItem
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string GuId { get; set; }
        public DateTime PublicationDate { get; set; }
        public int Type { get; set; } // 0 = Error, 1 = Maintenance, -1 = Undefined
        public string Description { get; set; }
    }
}
