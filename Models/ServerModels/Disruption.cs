using System;
using System.Collections.Generic;

namespace AvansApp.Models.ServerModels
{
    public class Disruption
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string TTL { get; set; }
        public string Discription { get; set; }
        public DateTime LastBuildTime { get; set; }
        public string Language { get; set; }
        public List<DisruptionItem> DisruptionItems { get; set; }

    }
}
