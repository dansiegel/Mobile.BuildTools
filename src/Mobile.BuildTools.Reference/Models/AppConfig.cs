using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema.Generation;

namespace Mobile.BuildTools.Models
{
    public class AppConfig : ToolItem
    {
        [Description("Configures the bundling strategy for advanced scenarios. By default it will only transform the app.config with the app.{BuildConfiguration}.config. You may optionally bundle all app.*.config or those that do not include Debug, Release, Store.")]
        [DefaultValue(AppConfigStrategy.TransformOnly)]
        [JsonProperty("strategy", NullValueHandling = NullValueHandling.Ignore)]
        public AppConfigStrategy Strategy { get; set; }
    }

    [JSchemaGenerationProvider(typeof(StringEnumGenerationProvider))]
    public enum AppConfigStrategy
    {
        TransformOnly,
        BundleAll,
        BundleNonStandard
    }
}
