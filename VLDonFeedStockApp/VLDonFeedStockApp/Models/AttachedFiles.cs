using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VLDonFeedStockApp.Models
{
    public class AttachedFiles
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
