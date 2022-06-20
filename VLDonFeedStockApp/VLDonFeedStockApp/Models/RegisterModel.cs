using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VLDonFeedStockApp.Models
{
    public class RegisterModel
    {
        [JsonProperty("workername")]
        public string WorkerName { get; set; }
        [JsonProperty("login")]
        public string Login { get; set; }
        [JsonProperty("organization")]
        public string Organization { get; set; }
        [JsonProperty("role")]
        public string Role { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("organizationid")]
        public int? OrganizationId { get; set; }
    }
}
