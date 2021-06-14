using System.ComponentModel;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class AutomaticVersioning : ToolItem
    {
#if SCHEMAGENERATOR
        [DefaultValue("PreferBuildNumber")]
        [System.ComponentModel.DataAnnotations.EnumDataType(typeof(VersionBehavior))]
        public string Behavior { get; set; }
        
        [DefaultValue("All")]
        [System.ComponentModel.DataAnnotations.EnumDataType(typeof(VersionEnvironment))]
        public string Environment { get; set; }
#else
        [JsonProperty("behavior")]
        public VersionBehavior Behavior { get; set; }

        [JsonProperty("environment")]
        public VersionEnvironment Environment { get; set; }
#endif
        [JsonProperty("versionOffset")]
        public int VersionOffset { get; set; }
    }
}
