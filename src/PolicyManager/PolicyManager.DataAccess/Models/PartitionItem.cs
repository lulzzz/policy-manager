using Newtonsoft.Json;

namespace PolicyManager.DataAccess.Models
{
    public class PartitionItem
    {
        [JsonProperty("partition")]
        public string Partition { get; set; }
    }
}
