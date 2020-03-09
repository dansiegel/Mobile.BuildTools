using System.ComponentModel;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class AppConfig : ToolItem
    {
        [DefaultValue(AppConfigStrategy.TransformOnly)]
        [JsonProperty("strategy")]
        public AppConfigStrategy Strategy { get; set; }
    }

    public enum AppConfigStrategy
    {
        TransformOnly,
        BundleAll,
        BundleNonStandard
    }
}
