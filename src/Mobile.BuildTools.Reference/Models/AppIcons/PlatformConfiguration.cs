using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Mobile.BuildTools.Models.AppIcons
{
    public class PlatformConfiguration : BaseImageConfiguration
    {
        [Description("Provides additional resource outputs for the specified source image.")]
        [JsonPropertyName("additionalOutputs")]
        public List<BaseImageConfiguration> AdditionalOutputs { get; set; }
    }
}
