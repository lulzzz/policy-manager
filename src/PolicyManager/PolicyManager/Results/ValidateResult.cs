using Newtonsoft.Json;

namespace PolicyManager.Results
{
    public class ValidateResult
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("policyName")]
        public string PolicyName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("result")]
        public string Result { get; set; }
    }
}
