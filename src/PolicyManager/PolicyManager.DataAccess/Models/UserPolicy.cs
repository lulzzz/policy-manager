using Newtonsoft.Json;
using PolicyManager.DataAccess.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PolicyManager.DataAccess.Models
{
    [DocumentName("UserPolicies")]
    public class UserPolicy
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("partition")]
        public string Partition
        {
            get { return UserPrincipalName?.Split('@').LastOrDefault(); }
        }

        [JsonProperty("userPrincipalName")]
        public string UserPrincipalName { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }

        [JsonProperty("modifiedDate")]
        public DateTime ModifiedDate { get; set; }

        [JsonProperty("userObjectId")]
        public string UserObjectId { get; set; }

        [JsonProperty("policyIds")]
        public IEnumerable<string> PolicyIds { get; set; }
    }
}
