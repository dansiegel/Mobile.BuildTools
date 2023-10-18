﻿using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Mobile.BuildTools.Models
{
    public class AppConfig : ToolItem
    {
        [Description("Configures the bundling strategy for advanced scenarios. By default it will only transform the app.config with the app.{BuildConfiguration}.config. You may optionally bundle all app.*.config or those that do not include Debug, Release, Store.")]
        [DefaultValue(AppConfigStrategy.TransformOnly)]
        [JsonPropertyName("strategy")]
        public AppConfigStrategy Strategy { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AppConfigStrategy
    {
        TransformOnly,
        BundleAll,
        BundleNonStandard
    }
}
