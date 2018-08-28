using Newtonsoft.Json;
using PolicyManager.DataAccess.Attributes;
using PolicyManager.DataAccess.Extensions;
using System;

namespace PolicyManager.DataAccess.Models
{
    [DocumentName("Policies")]
    public class PolicyRule
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("partition")]
        public string Partition
        {
            get { return Category.ToPolicyRulePartitionKey(); }
        }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }

        [JsonProperty("modifiedDate")]
        public DateTime ModifiedDate { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("rule")]
        public string Rule { get; set; }
    }
}
