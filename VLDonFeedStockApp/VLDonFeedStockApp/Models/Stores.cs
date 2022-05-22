using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VLDonFeedStockApp.Models
{
    public class Stores
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("relatedorganization")]
        public string RelatedOrganization { get; set; }
    }
}
