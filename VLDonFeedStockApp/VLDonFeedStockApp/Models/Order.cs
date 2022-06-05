using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VLDonFeedStockApp.Models
{
    public class Order
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("paper")]
        public bool Paper { get; set; }
        [JsonProperty("carton")]
        public bool Carton { get; set; }
        [JsonProperty("poddon")]
        public bool Poddon { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("createddata")]
        public string CreatedData { get; set; }
        [JsonProperty("takendata")]
        public string TakenData { get; set; }
        [JsonProperty("weighteddata")]
        public string WeightedData { get; set; }
        [JsonProperty("finisheddata")]
        public string FinishedData { get; set; }
        [JsonProperty("totalprice")]
        public string TotalPrice { get; set; }
    }
}
