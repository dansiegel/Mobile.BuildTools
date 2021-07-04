using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mobile.BuildTools.Models.Settings
{
    public class ValueConfig
    {
        [JsonProperty("name", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("type", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
        public PropertyType PropertyType { get; set; }

        [JsonProperty("isArray", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool IsArray { get; set; }

        [JsonProperty("defaultValue", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string DefaultValue { get; set; }
    }
}
