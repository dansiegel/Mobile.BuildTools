using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mobile.BuildTools.Models.Secrets
{
    public class ValueConfig
    {
        [JsonProperty("name")]
        public string Name { get; set; }

#if SCHEMAGENERATOR
        [System.ComponentModel.DataAnnotations.EnumDataType(typeof(PropertyType))]
        public string PropertyType { get; set; }
#else
        [JsonProperty("type")]
        public PropertyType PropertyType { get; set; }
#endif

        [JsonProperty("isArray")]
        public bool IsArray { get; set; }
    }
}
