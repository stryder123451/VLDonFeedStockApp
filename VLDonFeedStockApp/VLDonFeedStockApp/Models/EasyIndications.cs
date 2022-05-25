using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VLDonFeedStockApp.Models
{
    public class EasyIndications
    {
        [JsonProperty("data")]
        public string Data { get; set; }
        [JsonProperty("materials")]
        public string Materials { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("relatedorganizationid")]
        public int RelatedOrganizationId { get; set; }
        [JsonProperty("relatedoperator")]
        public string RelatedOperator { get; set; }
        [JsonProperty("organization")]
        public string Organization { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
