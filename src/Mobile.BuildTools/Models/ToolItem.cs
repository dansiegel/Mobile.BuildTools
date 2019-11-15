using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public abstract class ToolItem
    {
        [JsonProperty("disable")]
        public bool Disable { get; set; }
    }
}
