using System.ComponentModel;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class AutomaticVersioning : ToolItem
    {
        [DefaultValue(VersionBehavior.PreferBuildNumber)]
        [Description("Sets the default behavior for versioning the app. By default the Mobile.BuildTools will attempt to use a Build number and will fallback to a timestamp.")]
        [JsonProperty("behavior", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
        public VersionBehavior Behavior { get; set; }

        [DefaultValue(VersionEnvironment.BuildHost)]
        [Description("Sets the default versioning environment. You can use this locally only, on build hosts only, or everywhere for a unique build number every time.")]
        [JsonProperty("environment", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
        public VersionEnvironment Environment { get; set; }

        [Description("If you need to offset from your build number, you may want to set the Version Offset to get a version that will work for you.")]
        [JsonProperty("versionOffset", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
        public int VersionOffset { get; set; }
    }
}
