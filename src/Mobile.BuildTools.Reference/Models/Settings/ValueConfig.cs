using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mobile.BuildTools.Models.Settings
{
    public class ValueConfig
    {
        [JsonProperty("name", Required = Required.AllowNull)]
        public string Name { get; set; }

        [JsonProperty("type", Required = Required.AllowNull)]
        public PropertyType PropertyType { get; set; }

        [JsonProperty("isArray", Required = Required.AllowNull)]
        public bool IsArray { get; set; }

        [JsonProperty("defaultValue", Required = Required.AllowNull)]
        public string DefaultValue { get; set; }
    }
}
