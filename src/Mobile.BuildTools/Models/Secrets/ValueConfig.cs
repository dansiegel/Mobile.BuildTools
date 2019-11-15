using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mobile.BuildTools.Models.Secrets
{
    public class ValueConfig
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("type")]
        public PropertyType PropertyType { get; set; }

        [JsonProperty("isArray")]
        public bool IsArray { get; set; }
    }
}
