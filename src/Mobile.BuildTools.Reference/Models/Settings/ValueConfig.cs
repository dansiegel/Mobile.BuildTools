using System.ComponentModel;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models.Settings
{
    public class ValueConfig
    {
        [Description("The property name of the value to generate")]
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [Description("The property type of the generated property")]
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public PropertyType PropertyType { get; set; }

        [Description("Forces the property to be an Array type")]
        [JsonProperty("isArray", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsArray { get; set; }

        [Description("Sets the default value of the property. Can be `null` or `default` to set the default value of the property type.")]
        [JsonProperty("defaultValue", NullValueHandling = NullValueHandling.Ignore)]
        public string DefaultValue { get; set; }
    }
}
