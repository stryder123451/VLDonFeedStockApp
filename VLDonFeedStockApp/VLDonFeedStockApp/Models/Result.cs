﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VLDonFeedStockApp.Models
{
    public class Result
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
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
        [JsonProperty("takendata")]
        public string TakenData { get; set; }
        [JsonProperty("weightdata")]
        public string WeightData { get; set; }
        [JsonProperty("finisheddata")]
        public string FinishedData { get; set; }
        [JsonProperty("whotook")]
        public string WhoTook { get; set; }
        [JsonProperty("whoweighed")]
        public string WhoWeighed { get; set; }
        [JsonProperty("whoclosed")]
        public string WhoClosed { get; set; }
        [JsonProperty("lastmodified")]
        public string LastModified { get; set; }
        [JsonProperty("price")]
        public string Price { get; set; }
        public string RuState { get; set; }
    }

}