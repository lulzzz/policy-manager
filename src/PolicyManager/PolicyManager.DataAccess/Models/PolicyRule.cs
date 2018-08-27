using Newtonsoft.Json;

namespace PolicyManager.DataAccess.Models
{
    public class PolicyRule
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("partition")]
        public string Partition
        {
            get { return DisplayName.Substring(0, 3); }
        }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("rule")]
        public string Rule { get; set; }

        [JsonProperty("action")]
        public Action Action { get; set; }
    }
}
