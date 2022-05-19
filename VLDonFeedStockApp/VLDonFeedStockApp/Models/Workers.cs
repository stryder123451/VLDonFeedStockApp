using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VLDonFeedStockApp.Models
{
    public class Workers
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int LocalId { get; set; }
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("login")]
        public string Login { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("organization")]
        public string Organization { get; set; }
        [JsonProperty("role")]
        public string Role { get; set; }
        [JsonProperty("organizationid")]
        public int OrganizationId { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
