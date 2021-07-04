using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mobile.BuildTools.Models.Settings
{
    public class ValueConfig
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public PropertyType PropertyType { get; set; }

        [JsonProperty("isArray", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsArray { get; set; }

        [JsonProperty("defaultValue", NullValueHandling = NullValueHandling.Ignore)]
        public string DefaultValue { get; set; }
    }
}
