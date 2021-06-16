using System.ComponentModel;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class AutomaticVersioning : ToolItem
    {
        [JsonProperty("behavior")]
        public VersionBehavior Behavior { get; set; }

        [JsonProperty("environment")]
        public VersionEnvironment Environment { get; set; }

        [JsonProperty("versionOffset")]
        public int VersionOffset { get; set; }
    }
}
