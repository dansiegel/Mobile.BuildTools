using System.ComponentModel;
using System.Text.Json.Serialization;
using Mobile.BuildTools.Models.Secrets;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Models.Settings
{
    public class ValueConfig
    {
        [Description("The property name of the value to generate")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Description("The property type of the generated property")]
        [JsonPropertyName("type")]
        public PropertyType PropertyType { get; set; }

        [Description("Forces the property to be an Array type")]
        [JsonPropertyName("isArray")]
        public bool? IsArray { get; set; }

        [Description("Sets the default value of the property. Can be `null` or `default` to set the default value of the property type.")]
        [JsonPropertyName("defaultValue")]
        public string DefaultValue { get; set; }


        [JsonConverter(typeof(PlatformArrayJsonConverter))]
        public Platform[] RequiredPlatforms { get; set; } = [];
    }
}
