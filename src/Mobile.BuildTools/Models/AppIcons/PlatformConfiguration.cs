using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models.AppIcons
{
    public class PlatformConfiguration : BaseImageConfiguration
    {
        [JsonProperty("additionalOutputs")]
        public List<BaseImageConfiguration> AdditionalOutputs { get; set; }
    }
}
