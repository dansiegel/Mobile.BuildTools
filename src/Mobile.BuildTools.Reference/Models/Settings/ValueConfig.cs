using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mobile.BuildTools.Models.Settings
{
    public class ValueConfig
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public PropertyType PropertyType { get; set; }

        [JsonProperty("isArray")]
        public bool IsArray { get; set; }

        [JsonProperty("defaultValue")]
        public string DefaultValue { get; set; }
    }
}
