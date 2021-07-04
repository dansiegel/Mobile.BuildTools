using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models.AppIcons
{
    public class PlatformConfiguration : BaseImageConfiguration
    {
        [Description("Provides additional resource outputs for the specified source image.")]
        [JsonProperty("additionalOutputs", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<BaseImageConfiguration> AdditionalOutputs { get; set; }
    }
}
