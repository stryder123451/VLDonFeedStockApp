using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VLDonFeedStockApp.Models
{
    public class ResponseModel
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("expiration")]
        public string Expiration { get; set; }
    }
}
